using System.Text.RegularExpressions;
using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Larp.Landing.Shared.MwFifth;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;

namespace Larp.Landing.Server.Import;

public class ExcelImportResult
{
}

public class ExcelImporter
{
    private readonly LarpContext _larpContext;

    private Dictionary<string, Event> _events = default!;
    private Dictionary<int, Account> _players = default!;
    private Dictionary<int, Character> _characters = default!;
    private string _gameId = default!;
    private readonly ILogger<ExcelImporter> _logger;
    private readonly MwFifthCharacterManager _characterManager;
    private GameState _gameState = default!;
    private DateTimeOffset _now;

    public ExcelImporter(LarpContext larpContext, ILogger<ExcelImporter> logger,
        MwFifthCharacterManager characterManager)
    {
        _larpContext = larpContext;
        _logger = logger;
        _characterManager = characterManager;
    }

    public async Task<ExcelImportResult> Import(string fileName)
    {
        _now = DateTimeOffset.Now;

        using var package = new ExcelPackage(fileName);
        using var workbook = package.Workbook;

        _gameState = await _larpContext.MwFifthGame.GetGameState();

        _gameId = await _larpContext.Games
            .Find(x => x.Name == GameState.GameName)
            .Project(x => x.Id)
            .FirstOrDefaultAsync();

        _players = (await _larpContext.Accounts
                .Find(x => x.ImportId.HasValue).ToListAsync())
            .ToDictionary(x => (int)x.ImportId!);
        _characters = (await _larpContext.MwFifthGame.Characters
                .Find(x => x.ImportId.HasValue).ToListAsync())
            .ToDictionary(x => (int)x.ImportId!);
        _events = (await _larpContext.Events
                .Find(x => x.ImportId != null).ToListAsync())
            .ToDictionary(x => x.ImportId!);

        await ProcessPlayers(workbook.Worksheets["Players"]);
        await ProcessCharacters(workbook.Worksheets["Characters"]);
        await ProcessEvents(workbook.Worksheets["Moonstones"]);
        await _characterManager.UpdateMoonstone();

        return new ExcelImportResult();
    }

    private class ExcelEvent
    {
        public string Name { get; }
        public Dictionary<int, int> Moonstone { get; } = new();

        public ExcelEvent(string name)
        {
            Name = name;
        }
    }

    private async Task ProcessEvents(ExcelWorksheet sheet)
    {
        var events = new List<WriteModel<Event>>();
        var attendances = new List<WriteModel<Attendance>>();

        for (var column = 4; column < sheet.Columns.EndColumn; column++)
        {
            var eventName = sheet.Cells[1, column].Value.ToString();
            if (string.IsNullOrWhiteSpace(eventName)) continue;
            var excelEvent = new ExcelEvent(eventName);

            for (var row = 2; row < sheet.Rows.EndRow; row++)
            {
                if (sheet.Cells[row, 1].Value is not double playerId) continue;
                if (sheet.Cells[row, column].Value is not double moonstone) continue;
                excelEvent.Moonstone.Add((int)playerId, (int)moonstone);
            }

            if (!_events.TryGetValue(eventName, out var @event))
            {
                if (!int.TryParse(eventName[0..4], out var year))
                    year = 2018;


                var date = new DateOnly(year, 1, 1);

                var type = "Game";
                if (eventName.Contains("Work", StringComparison.InvariantCultureIgnoreCase))
                    type = "Workday";
                if (eventName.Contains("Setup", StringComparison.InvariantCultureIgnoreCase))
                    type = "Workday";
                if (eventName.Contains("Patreon"))
                    type = "Subscription";
                if (eventName.Contains("Nitro"))
                    type = "Subscription";
                if (eventName.Contains("Trivia"))
                    type = "Contest";
                if (eventName.Contains("Recruit") || eventName.Contains("Retirement") || eventName.Contains("Paper") ||
                    eventName.Contains("Other") || eventName.Contains("Character"))
                    type = "Other";

                @event = new Event()
                {
                    EventId = ObjectId.GenerateNewId().ToString(),
                    Date = date,
                    ImportId = eventName,
                    Title = eventName,
                    GameId = _gameId,
                    EventType = type,
                    IsHidden = type is "Subscription" or "Contest" or "Other",
                };

                var components = new List<EventComponent>();
                if (type == "Game")
                {
                    var match = Regex.Match(eventName, @"(\d+)\/(\d+)");
                    if (match.Success)
                    {
                        @event.Date = date = new DateOnly(year,
                            int.TryParse(match.Groups[1].Value, out var month) ? month : 1,
                            int.TryParse(match.Groups[2].Value, out var day) ? day : 1
                        );
                    }
                    else
                    {
                        match = Regex.Match(eventName, @"(June|July|Aug) (\d+)");
                        var month = match.Groups[1].Value switch
                        {
                            "June" => 6,
                            "July" => 7,
                            "Aug" => 8,
                            _ => 1
                        };
                        @event.Date = date = new DateOnly(year,
                            month,
                            int.TryParse(match.Groups[2].Value, out var day) ? day : 1
                        );
                    }

                    if (eventName.Contains("Burgundar") || eventName.Contains("Keep"))
                    {
                        @event.EventCost = 20;
                        @event.ChronicleCost = 15;
                        components.AddRange(new[]
                        {
                            new EventComponent()
                            {
                                Name = "Friday", Date = date, ComponentId = "F", Free = true
                            },
                            new EventComponent()
                            {
                                Name = "Chronicle 1", Date = date.AddDays(1), ComponentId = "1"
                            },
                            new EventComponent()
                            {
                                Name = "Chronicle 2", Date = date.AddDays(1), ComponentId = "2"
                            },
                            new EventComponent()
                            {
                                Name = "Chronicle 3", Date = date.AddDays(2), ComponentId = "3"
                            }
                        });
                    }
                    else if (eventName.Contains("Novgorond"))
                    {
                        @event.EventCost = 20;
                        @event.ChronicleCost = 20;
                        components.AddRange(new[]
                        {
                            new EventComponent()
                            {
                                Name = "First Half", Date = date, ComponentId = "1",
                            },
                            new EventComponent()
                            {
                                Name = "Second Half", Date = date, ComponentId = "2"
                            }
                        });
                    }
                }

                @event.Components = components.ToArray();

                events.Add(new InsertOneModel<Event>(@event));
            }

            foreach (var (playerId, moonstone) in excelEvent.Moonstone)
            {
                if (!_players.TryGetValue(playerId, out var account)) continue;

                var attendance = new ReplaceOneModel<Attendance>(
                    Builders<Attendance>.Filter.And(
                        Builders<Attendance>.Filter.Eq(x => x.AccountId, account.AccountId),
                        Builders<Attendance>.Filter.Eq(x => x.EventId, @event.EventId)
                    ), new Attendance()
                    {
                        // Id = ObjectId.GenerateNewId().ToString(),
                        EventId = @event.EventId,
                        AccountId = account.AccountId,
                        MwFifth = new MwFifthAttendance() { Moonstone = moonstone }
                    }) { IsUpsert = true };
                attendances.Add(attendance);
            }
        }

        if (events.Count > 0)
            await _larpContext.Events.BulkWriteAsync(events);
        if (attendances.Count > 0)
            await _larpContext.Attendances.BulkWriteAsync(attendances);
    }

    private async Task ProcessPlayers(ExcelWorksheet sheet)
    {
        var updates = new List<InsertOneModel<Account>>();

        for (var row = 2; row < sheet.Rows.EndRow; row++)
        {
            var playerId = (double?)sheet.Cells[row, 1].Value;
            var name = (string?)sheet.Cells[row, 2].Value;
            var email = (string?)sheet.Cells[row, 3].Value;
            var dob = sheet.Cells[row, 4].Value;
            var notes = (string?)sheet.Cells[row, 5].Value;

            if (playerId == null || name == null) continue;

            var importId = (int)playerId.Value;

            if (_players.ContainsKey(importId)) continue;

            var birthDate = dob is double dobDate
                ? (DateOnly?)DateOnly.FromDateTime(DateTime.FromOADate(dobDate))
                : null;

            var account = new Account()
            {
                AccountId = ObjectId.GenerateNewId().ToString(),
                ImportId = importId,
                Created = _now,
                Name = name,
                Notes = notes,
                BirthDate = birthDate,
                LastUpdate = _now
            };

            if (email != null)
            {
                account.Emails.Add(new AccountEmail()
                {
                    Email = email,
                    NormalizedEmail = email.ToLowerInvariant(),
                    IsPreferred = true,
                    IsVerified = false
                });
            }

            updates.Add(new InsertOneModel<Account>(account));
            _players.Add(importId, account);
        }

        if (updates.Count > 0)
            await _larpContext.Accounts.BulkWriteAsync(updates);
    }

    private async Task ProcessCharacters(ExcelWorksheet sheet)
    {
        var characters = new List<InsertOneModel<Character>>();
        var revisions = new List<InsertOneModel<CharacterRevision>>();

        var playerNames = _players.Values
            .DistinctBy(x => x.Name)
            .Where(x => x.Name != null)
            .ToDictionary(x => x.Name!, x => x, comparer: StringComparer.InvariantCultureIgnoreCase);

        static int ToInt(object? value) => (int)(double)(value ?? 0);

        for (var row = 2; row < sheet.Rows.EndRow; row++)
        {
            var characterId = sheet.Cells[row, 1].Value;
            if (characterId == null) continue;

            var characterName = (string)sheet.Cells[row, 4].Value;
            if (characterName == "Lord Example") continue;

            var importId = ToInt(characterId);
            if (_characters.ContainsKey(importId)) continue;

            var playerName = (string?)sheet.Cells[row, 5].Value;
            if (playerName == null || !playerNames.TryGetValue(playerName, out var account))
            {
                _logger.LogWarning(
                    "Unable to import character {ImportId} {CharacterName} because no account with name {AccountName} was found",
                    importId, characterName, playerName);
                continue;
            }

            var character = new Character
            {
                CharacterId = ObjectId.GenerateNewId().ToString(),
                ImportId = importId,
                AccountId = account!.AccountId,
                CharacterName = characterName,
                CreatedOn = _now,
                ImportedMoonstone = ToInt(sheet.Cells[row, 2].Value),
                State = CharacterState.Live
            };

            var homeChapter = TranslateHomeChapter((string)sheet.Cells[row, 6].Value);
            var religion = TranslateReligion((string)sheet.Cells[row, 10].Value);
            var skills = TranslateSkills((string?)sheet.Cells[row, 20].Value);
            var advantages = TranslateVantages((string)sheet.Cells[row, 24].Value);
            var disadvantages = TranslateVantages((string)sheet.Cells[row, 25].Value);
            var spells = TranslateList((string)sheet.Cells[row, 26].Value);

            if (homeChapter == null)
            {
                _logger.LogWarning(
                    "Unable to import character {ImportId} {CharacterName} because no home chapter was provided",
                    importId, characterName);
            }

            TranslateOccupation((string?)sheet.Cells[row, 7].Value, out var occupation, out var occupationSkills);
            TranslateOccupation((string?)sheet.Cells[row, 8].Value, out var enhancement, out var enhancementSkills);

            var revision = new CharacterRevision
            {
                RevisionId = ObjectId.GenerateNewId().ToString(),
                AccountId = account!.AccountId,
                CharacterId = character.CharacterId,
                State = CharacterState.Live,
                CreatedOn = _now,
                ApprovedOn = _now,
                SubmittedOn = _now,
                CharacterName = characterName,
                HomeChapter = homeChapter,
                Religion = religion,
                NoOccupation = occupation == null,
                Homeland = (string)sheet.Cells[row, 9].Value,
                Courage = ToInt(sheet.Cells[row, 11].Value),
                Dexterity = ToInt(sheet.Cells[row, 12].Value),
                Empathy = ToInt(sheet.Cells[row, 13].Value),
                Passion = ToInt(sheet.Cells[row, 14].Value),
                Prowess = ToInt(sheet.Cells[row, 15].Value),
                Wisdom = ToInt(sheet.Cells[row, 16].Value),
                Cures = (string)sheet.Cells[row, 27].Value,
                Documents = (string)sheet.Cells[row, 28].Value,
                PublicHistory = (string)sheet.Cells[row, 29].Value,
                PrivateHistory = (string)sheet.Cells[row, 30].Value,
                UnusualFeatures = (string)sheet.Cells[row, 31].Value,
                Spells = spells,
                Advantages = advantages,
                Disadvantages = disadvantages,
                AgeGroup = AgeGroup.Adult
            };

            revision.NoHistory = string.IsNullOrWhiteSpace(revision.PublicHistory);
            revision.NoAdvantages = revision.Advantages.Length == 0 && revision.Disadvantages.Length == 0;
            revision.NoOccupation = occupation == null;

            var builder = new CharacterBuilder(
                new CharacterAndRevision(character, revision, new MoonstoneInfo(0, 0)),
                _gameState, NullLogger.Instance)
            {
                Occupation = occupation,
                Enhancement = enhancement,
                OccupationalChosenSkills = occupationSkills.Concat(enhancementSkills).ToArray(),
                PurchasedSkills = skills
            };
            builder.UpdateMoonstone();

            character.PreregistrationRevisionNotes = revision.PreregistrationNotes;

            character.UsedMoonstone = revision.TotalMoonstone;
            characters.Add(new InsertOneModel<Character>(character));
            revisions.Add(new InsertOneModel<CharacterRevision>(revision));
        }

        if (characters.Count > 0)
            await _larpContext.MwFifthGame.Characters.BulkWriteAsync(characters);
        if (revisions.Count > 0)
            await _larpContext.MwFifthGame.CharacterRevisions.BulkWriteAsync(revisions);
    }

    private CharacterVantage[] TranslateVantages(string? value)
    {
        if (value == null) return Array.Empty<CharacterVantage>();

        return value.Split(',').Select(x => x.Trim()).Select(item =>
        {
            if (int.TryParse(item[0..1], out var rank))
            {
                var name = item[2..];
                return new CharacterVantage() { Name = name, Rank = rank };
            }

            var vantage = _gameState.Advantages.FirstOrDefault(x => x.Name == item)
                          ?? _gameState.Disadvantages.FirstOrDefault(x => x.Name == item);

            if (vantage != null)
                return new CharacterVantage() { Name = vantage.Name, Rank = vantage.Rank };

            _logger.LogWarning("Vantage \"{VantageName}\" was not found", item);
            return new CharacterVantage() { Name = item, Rank = 0 };
        }).ToArray();
    }

    private static string[] TranslateList(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(',').Select(x => x.Trim()).ToArray();

    private void TranslateOccupation(string? value, out string? name, out string[] skills)
    {
        name = null;
        skills = Array.Empty<string>();

        if (value == null)
            return;

        var match = Regex.Match(value, @"^([\w\- /]+)( \((.*)\))?$", RegexOptions.Compiled);

        if (!match.Success)
            return;

        name = match.Groups[1].Value;
        skills = match.Groups[3].Value.Split(',')
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }

    private CharacterSkill[] TranslateSkills(string? value)
    {
        if (value == null) return Array.Empty<CharacterSkill>();

        return value.Split(',')
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x) && x != "-")
            .Select(item =>
            {
                string name;
                int count = 1;
                var match = Regex.Match(item, @"^(.*) x(\d+)", RegexOptions.Compiled);
                if (match.Success)
                {
                    name = match.Groups[1].Value;
                    count = int.TryParse(match.Groups[2].Value, out var i) ? i : 1;
                }
                else
                {
                    match = Regex.Match(item, @"^(.*) \d", RegexOptions.Compiled);
                    if (!match.Success)
                        throw new Exception($"Skill \"{item}\" is not properly formatted");
                    name = match.Groups[1].Value;
                    count = 1;
                }

                var skill = _gameState.Skills.FirstOrDefault(x =>
                    string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                if (skill == null)
                    throw new Exception($"Skill \"{name}\" does not exist");

                if (skill.Purchasable == SkillPurchasable.Once && count > 1)
                    count = 1;

                return new CharacterSkill()
                {
                    Name = skill.Name,
                    Purchases = count,
                    Rank = (skill.RanksPerPurchase ?? 0) * count,
                    Type = SkillPurchase.Purchased
                };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .ToArray();
    }

    private static string TranslateReligion(string? value)
    {
        if (value == null)
            return "none";
        if (value.Contains("wild", StringComparison.InvariantCultureIgnoreCase))
            return "wild";
        if (value.Contains("mercy", StringComparison.InvariantCultureIgnoreCase))
            return "mercy";
        if (value.Contains("justice", StringComparison.InvariantCultureIgnoreCase))
            return "justice";
        if (value.Contains("all", StringComparison.InvariantCultureIgnoreCase))
            return "wild";
        if (value.Contains("chaos", StringComparison.InvariantCultureIgnoreCase))
            return "chaos";
        if (value.Contains("old", StringComparison.InvariantCultureIgnoreCase))
            return "old";
        return "none";
    }

    private static string? TranslateHomeChapter(string? homeChapter)
    {
        if (homeChapter == null)
            return null;
        if (homeChapter.StartsWith("The Keep", StringComparison.InvariantCultureIgnoreCase) ||
            homeChapter.StartsWith("The Mystwood Keep", StringComparison.InvariantCultureIgnoreCase))
            return "keep";
        if (homeChapter.StartsWith("Burgundar", StringComparison.InvariantCultureIgnoreCase))
            return "burgundar";
        if (homeChapter.StartsWith("Albion", StringComparison.InvariantCultureIgnoreCase))
            return "albion";
        if (homeChapter.StartsWith("Novgorond", StringComparison.InvariantCultureIgnoreCase))
            return "novgorond";
        return homeChapter;
    }
}