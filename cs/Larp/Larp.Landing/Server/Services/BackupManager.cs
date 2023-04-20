using Larp.Data.MwFifth;
using Larp.Data.Seeder;
using Larp.Landing.Server.Import;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Larp.Landing.Server.Services;

public class BackupManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupManager> _logger;
    private readonly LarpContext _larpContext;

    public BackupManager(IServiceProvider serviceProvider, ILogger<BackupManager> logger, LarpContext larpContext)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _larpContext = larpContext;
    }

    public async Task<StringResult> Import(Stream data)
    {
        var fileProvider = _serviceProvider.GetRequiredService<IFileProvider>();
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var path = fileProvider.GetFileInfo($"{Path.GetRandomFileName()}.xlsx");
            var file = new FileInfo(path.PhysicalPath!);

            await using (var fileStream = File.OpenWrite(file.FullName))
            {
                await data.CopyToAsync(fileStream);
            }

            var importer = _serviceProvider.GetRequiredService<ExcelImporter>();
            await importer.Import(file.FullName);

            return StringResult.Success("Import successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import file");
            return StringResult.Failed("Failed to import file due to exception: " + ex.Message);
        }
    }

    public async Task<IFileInfo> Export()
    {
        var fileProvider = _serviceProvider.GetRequiredService<IFileProvider>();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var path = fileProvider.GetFileInfo($"{Path.GetRandomFileName()}.xlsx");

        var file = new FileInfo(path.PhysicalPath!);
        using var package = new ExcelPackage(file);

        using var workbook = package.Workbook;

        var players = (await _larpContext.Accounts
                .Find(_ => true)
                .ToListAsync())
            .ToDictionary(x => x.AccountId);

        var playerSheet = workbook.Worksheets.Add("Players");
        {
            playerSheet.Cells.LoadFromCollection(players.Values.Select(p => new
            {
                PlayerId = p.AccountId,
                PlayerName = p.Name,
                p.Phone,
                p.Emails.FirstOrDefault(x => x.IsPreferred)?.Email,
                p.Location,
                p.Notes,
                PreregistrationNotes = p.MwFifthPreregistrationNotes,
                TotalMoonstone = p.MwFifthMoonstone,
                UsedMoonstone = p.MwFifthMoonstone,
                UnspentMoonstone = p.MwFifthMoonstone - p.MwFifthUsedMoonstone,
            }), true, TableStyles.Light6);
        }

        var characterSheet = workbook.Worksheets.Add("Characters");
        {
            var characters = await _larpContext.MwFifthGame.CharacterRevisions
                .Find(x => x.State == CharacterState.Live)
                .ToListAsync();
            characterSheet.Cells.LoadFromCollection(characters.Select(character =>
            {
                var account = players[character.AccountId];

                return new
                {
                    CharacterId = character.RevisionId,
                    PlayerId = character.AccountId,
                    PlayerTotalMoonstone = account.MwFifthMoonstone,
                    PlayerUnspentMoonstone = account.MwFifthMoonstone - account.MwFifthUsedMoonstone,
                    character.CharacterName,
                    PlayerName = account.Name ?? "No Name Set",
                    character.HomeChapter,
                    character.Occupation,
                    character.Specialty,
                    OccupationalEnhancement = character.Enhancement,
                    character.Homeland,
                    character.Religion,
                    character.Courage,
                    character.Dexterity,
                    character.Empathy,
                    character.Passion,
                    character.Prowess,
                    character.Wisdom,
                    character.GiftMoonstone,
                    OccupationalSkills = SkillList(character.Skills.Where(x =>
                        x.Type is SkillPurchase.Occupation or SkillPurchase.OccupationChoice)),
                    PurchasedSkills = SkillList(character.Skills.Where(x => x.Type is SkillPurchase.Purchased)),
                    character.SkillMoonstone,
                    SkillCount = character.Skills.Where(x => x.Type is SkillPurchase.Purchased)
                        .Sum(x => Math.Min(1, x.Purchases ?? 1)),
                    FreeSkills =
                        SkillList(character.Skills.Where(x => x.Type is SkillPurchase.Free or SkillPurchase.Bestowed)),
                    Advantages = VantageList(character.Advantages),
                    Disadvantages = VantageList(character.Disadvantages),
                    Spells = string.Join(", ", character.Spells),
                    character.Cures,
                    character.Documents,
                    character.PublicHistory,
                    character.PrivateHistory,
                    character.UnusualFeatures,
                    CharacterNotes = character.Notes,
                    PlayerNotes = account.Notes,
                    CharacterPreregistrationNotes = character.PreregistrationNotes,
                    PlayerPreregistrationNotes = account.MwFifthPreregistrationNotes,
                };
            }), true, TableStyles.Light6);
        }

        var moonstonesSheet = workbook.Worksheets.Add("Moonstones");
        {
            var events = await _larpContext.Events.Find(_ => true).ToListAsync();
            var attendances = (await _larpContext.Attendances.AsQueryable()
                    .GroupBy(x => x.EventId)
                    .ToListAsync())
                .ToDictionary(x => x.Key, x => x.ToArray());

            moonstonesSheet.Cells[1, 1].Value = "ID";
            moonstonesSheet.Cells[1, 2].Value = "Player";
            moonstonesSheet.Cells[1, 3].Value = "Total MS";

            var row = 2;
            foreach (var player in players.Values)
            {
                player.ImportId = row;
                moonstonesSheet.Cells[row, 1].Value = player.AccountId;
                moonstonesSheet.Cells[row, 2].Value = player.Name ?? "No Name Set";
                moonstonesSheet.Cells[row, 3].Value = player.MwFifthMoonstone;
                row++;
            }

            var column = 4;
            foreach (var @event in events)
            {
                moonstonesSheet.Cells[1, column].Value = @event.Title;
                foreach (var attendance in attendances.GetValueOrDefault(@event.EventId) ?? Array.Empty<Attendance>())
                {
                    var player = players[attendance.AccountId];
                    moonstonesSheet.Cells[player.ImportId ?? 0, column].Value = @attendance.MwFifth?.Moonstone;
                }

                column++;
            }

            moonstonesSheet.Cells[1, 1, row - 1, 1].Style.Font.Bold = true;
            moonstonesSheet.Cells[1, 1, 1, column - 1].Style.Font.Bold = true;
            moonstonesSheet.Cells[1, 1, 1, column - 1].Style.TextRotation = 90;
            moonstonesSheet.Columns[1, column - 1].AutoFit();
        }

        string SkillList(IEnumerable<CharacterSkill> skills) =>
            string.Join(", ", skills.Select(x => x.Title));

        string VantageList(IEnumerable<CharacterVantage> vantages) =>
            string.Join(", ", vantages.Select(x => x.Title));

        await package.SaveAsync();

        return path;
    }

    async Task<Dictionary<string, AccountName>> GetAccountNames()
    {
        var names = await _larpContext.Accounts.Find(_ => true)
            .Project(account => new AccountName
            {
                AccountId = account.AccountId,
                State = account.State,
                Name = account.Name
            })
            .ToListAsync();
        return names.ToDictionary(x => x.AccountId);
    }

    public async Task<IFileInfo> ExportLetters(string eventId)
    {
        var letterManager = _serviceProvider.GetRequiredService<LetterManager>();
        var eventManager = _serviceProvider.GetRequiredService<EventManager>();
        var fileProvider = _serviceProvider.GetRequiredService<IFileProvider>();

        var info = await letterManager.GetByEvent(eventId);
        var @event = info.Events.First().Value;
        var attendance = await eventManager.GetEventAttendances(eventId);
        var accountNames = await GetAccountNames();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var path = fileProvider.GetFileInfo($"{Path.GetRandomFileName()}.xlsx");

        var playerList = @attendance.Select(x => x.AccountId).Distinct().ToList();
        var accounts = await _larpContext.Accounts.Find(x => playerList.Contains(x.AccountId)).ToListAsync();
        var characters = await _larpContext.MwFifthGame.CharacterRevisions.Find(x => playerList.Contains(x.AccountId))
            .ToListAsync();

        var file = new FileInfo(path.PhysicalPath!);
        using var package = new ExcelPackage(file);

        using var workbook = package.Workbook;

        var attendanceSheet = workbook.Worksheets.Add("Attendance");
        {
            attendanceSheet.Cells.LoadFromCollection(attendance.Select(p => new
            {
                PlayerId = p.AccountId,
                PlayerName = accountNames.TryGetValue(p.AccountId, out var player) ? player.Name : "No Name Set",
                Moonstone = p.MwFifth?.Moonstone
            }), true, TableStyles.Light6);
        }

        foreach (var letters in info.Letters.GroupBy(x => x.Value.TemplateId, x => x.Value))
        {
            var template = info.LetterTemplates[letters.Key];

            var sheet = workbook.Worksheets.Add(template.Name);

            int row = 1, column = 1;

            void Write(string? value) => sheet.Cells[row, column++].Value = value;

            void NextRow()
            {
                row++;
                column = 1;
            }

            var fields = template.Fields.Select(x => x.Name).ToArray();
            Write("Player Name");
            foreach (var field in fields)
            {
                Write(field);
            }

            NextRow();

            foreach (var letter in letters)
            {
                var name = accountNames.TryGetValue(letter.AccountId, out var player) ? player.Name : "No Name Set";
                Write(name);
                foreach (var field in fields)
                {
                    Write(letter.Fields.TryGetValue(field, out var value) ? value : null);
                }

                NextRow();
            }

            sheet.Cells[1, 1, row, column].AutoFitColumns();
        }

        await package.SaveAsync();

        return path;
    }

    public async Task Reseed()
    {
        var seeder = _serviceProvider.GetRequiredService<LarpDataSeeder>();
        await seeder.Reseed();
    }
}