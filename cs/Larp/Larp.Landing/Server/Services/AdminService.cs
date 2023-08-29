using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Larp.Data.MwFifth;
using Larp.Data.Seeder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Larp.Landing.Server.Services;

public class AdminService : IAdminService
{
    private readonly Account _account;
    private readonly AttachmentManager _attachmentManager;
    private readonly BackupManager _backupManager;
    private readonly MwFifthCharacterManager _characterManager;
    private readonly CitationManager _citationManager;
    private readonly ISystemClock _clock;
    private readonly LarpContext _db;
    private readonly EventManager _eventManager;
    private readonly LetterManager _letterManager;
    private readonly string? _sessionId;
    private readonly IUserSessionManager _userSessionManager;

    public AdminService(LarpContext db, IUserSession userSession,
        MwFifthCharacterManager characterManager, IUserSessionManager userSessionManager,
        LetterManager letterManager, EventManager eventManager, CitationManager citationManager,
        AttachmentManager attachmentManager, BackupManager backupManager, ISystemClock clock)
    {
        _db = db;
        _characterManager = characterManager;
        _userSessionManager = userSessionManager;
        _letterManager = letterManager;
        _eventManager = eventManager;
        _citationManager = citationManager;
        _attachmentManager = attachmentManager;
        _backupManager = backupManager;
        _clock = clock;
        _account = userSession.Account!;
        _sessionId = userSession.SessionId;
    }

    public async Task<Account[]> GetAccounts(AccountState accountState)
    {
        var attachments = (await _db.AccountAttachments.AsQueryable()
                .GroupBy(x => x.AccountId)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        var attendances = (await _db.Attendances.AsQueryable()
                .Join(_db.Events.AsQueryable(), a => a.EventId, e => e.EventId, (a, e) =>
                    new
                    {
                        Attendance = a,
                        Event = e
                    })
                .Where(x => x.Event.EventType == "Game")
                .GroupBy(x => x.Attendance.AccountId)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        var characters = (await _db.MwFifthGame.Characters.AsQueryable()
                .Where(x => x.State == CharacterState.Live)
                .GroupBy(x => x.AccountId)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        var citations = (await _db.Citations.AsQueryable()
                .Where(x => x.State == CitationState.Open)
                .GroupBy(x => x.AccountId)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        var accounts = await _db.Accounts
            .Find(account => account.State == accountState)
            .ToListAsync();
        accounts.ForEach(account =>
        {
            account.AttachmentCount = attachments.GetValueOrDefault(account.AccountId);
            account.AttendanceCount = attendances.GetValueOrDefault(account.AccountId);
            account.CitationCount = citations.GetValueOrDefault(account.AccountId);
            account.CharacterCount = characters.GetValueOrDefault(account.AccountId);
        });
        return accounts.ToArray();
    }

    public async Task<IFileInfo> ExportOccupations()
    {
        var gameState = await _db.MwFifthGame.GetGameState();
        var memory = new MemoryStream();
        var options = new JsonSerializerOptions(LarpDataSeeder.JsonSerializerOptions) { WriteIndented = true };
        await JsonSerializer.SerializeAsync(memory, gameState.Occupations, options);
        memory.Seek(0, SeekOrigin.Begin);
        return new DownloadFileInfo(memory, "Mw5eOccupations.json", (int)memory.Length);
    }

    public Task<IFileInfo> ExportLetters(string eventId) =>
        _backupManager.ExportLetters(eventId);

    async Task IAdminService.MergeAccounts(string fromAccountId, string toAccountId)
    {
        await Log(new AccountMergeLogEvent { FromAccountId = fromAccountId, ToAccountId = toAccountId });

        await _db.Sessions.UpdateManyAsync(x => x.AccountId == fromAccountId,
            Builders<Session>.Update.Set(x => x.AccountId, toAccountId));

        await _db.AccountAttachments.UpdateManyAsync(x => x.AccountId == fromAccountId,
            Builders<AccountAttachment>.Update.Set(x => x.AccountId, toAccountId));

        await _db.AccountAttachments.UpdateManyAsync(x => x.UploadedBy == fromAccountId,
            Builders<AccountAttachment>.Update.Set(x => x.UploadedBy, toAccountId));

        await _db.Letters.UpdateManyAsync(x => x.AccountId == fromAccountId,
            Builders<Letter>.Update.Set(x => x.AccountId, toAccountId));

        await _db.Letters.UpdateManyAsync(x => x.ApprovedBy == fromAccountId,
            Builders<Letter>.Update.Set(x => x.ApprovedBy, toAccountId));

        await _characterManager.MoveAll(fromAccountId, toAccountId);

        var attendances = await _db.Attendances
            .Find(attendance => attendance.AccountId == fromAccountId).ToListAsync();

        foreach (var attendance in attendances)
        {
            await SetEventAttendance(attendance.EventId, toAccountId, true,
                attendance.MwFifth?.Moonstone, attendance.MwFifth?.PostMoonstone,
                attendance.ProvidedPayment, attendance.ExpectedPayment,
                attendance.MwFifth?.CharacterIds ?? Array.Empty<string>());
        }

        await _db.Attendances.DeleteManyAsync(attendance => attendance.AccountId == fromAccountId);

        var fromAccount = await GetAccount(fromAccountId);
        var toAccount = await GetAccount(toAccountId);

        await UpdateAccount(toAccountId,
            toAccount.Name ?? fromAccount.Name,
            toAccount.Location ?? fromAccount.Location,
            toAccount.Phone ?? fromAccount.Phone,
            toAccount.BirthDate ?? fromAccount.BirthDate,
            $"{fromAccount.Notes ?? ""} {toAccount.Notes ?? ""}".Trim(),
            toAccount.DiscountPercent ?? fromAccount.DiscountPercent
        );

        await ArchiveAccount(fromAccountId);

        foreach (var email in fromAccount.Emails)
            await _userSessionManager.AddEmailAddress(toAccountId, email.Email);
    }

    public async Task AddAccountEmail(string accountId, string email)
    {
        await _userSessionManager.AddEmailAddress(accountId, email);
    }

    public async Task RemoveAccountEmail(string accountId, string email)
    {
        await _userSessionManager.RemoveEmailAddress(accountId, email);
    }

    public async Task<LetterTemplate> DraftLetterTemplate() =>
        await _letterManager.DraftTemplate();

    public async Task SaveLetterTemplate(string templateId, LetterTemplate template)
    {
        template.LetterTemplateId = templateId;
        await _letterManager.SaveTemplate(template);
    }

    public async Task<LetterTemplate[]> GetLetterTemplates() =>
        await _letterManager.GetTemplates();

    public async Task<LetterTemplate[]> GetLetterTemplateNames() =>
        await _letterManager.GetTemplateNames();

    public async Task<LetterTemplate> GetLetterTemplate(string templateId) =>
        await _letterManager.GetTemplate(templateId);

    public async Task ApproveLetter(string letterId) =>
        await _letterManager.Approve(letterId, _account.AccountId);

    public async Task RejectLetter(string letterId) =>
        await _letterManager.Reject(letterId, _account.AccountId);

    public async Task UnapproveLetter(string letterId) =>
        await _letterManager.Unapprove(letterId, _account.AccountId);

    public async Task<Letter[]> GetSubmittedLetters() =>
        await _letterManager.GetByState(LetterState.Submitted);

    public async Task<EventsAndLetters> GetEventLetters(string eventId) =>
        await _letterManager.GetByEvent(eventId);

    public async Task<Letter[]> GetTemplateLetters(string templateId) =>
        await _letterManager.GetByTemplate(templateId);

    public async Task SaveAttachment(string attachmentId, AccountAttachment attachment) =>
        await _attachmentManager.SaveAttachment(attachmentId, attachment);

    async Task<AccountAttachment> IAdminService.GetAttachment(string attachmentId) =>
        await _attachmentManager.GetAttachment(attachmentId);

    public async Task DeleteAttachment(string attachmentId) =>
        await _attachmentManager.DeleteAttachment(attachmentId);

    async Task<AccountAttachment[]> IAdminService.GetAccountAttachments(string accountId) =>
        await _attachmentManager.GetAccountAttachments(accountId);

    public async Task<StringResult> Attach(string accountId, Stream data, string fileName, string mediaType) =>
        await _attachmentManager.Attach(accountId, data, fileName, mediaType, _account.AccountId);

    public async Task ApproveMwFifthCharacter(string characterId) =>
        await _characterManager.Approve(characterId, _account.AccountId);

    public async Task RejectMwFifthCharacter(string characterId, string? reviewerNotes) =>
        await _characterManager.Reject(characterId, reviewerNotes);

    public async Task<CharacterAndRevision> ReviseMwFifthCharacter(string characterId) =>
        await _characterManager.GetDraft(characterId, _account, true);

    public async Task SaveMwFifthCharacter(CharacterRevision revision) =>
        await _characterManager.Save(revision, _account, true);

    public async Task DeleteMwFifthCharacter(string characterId) =>
        await _characterManager.Delete(characterId, _account, true);

    public async Task MoveMwFifthCharacter(string characterId, string newAccountId) =>
        await _characterManager.Move(characterId, newAccountId);

    public async Task<Event> DraftEvent() => await _eventManager.DraftEvent();

    public async Task SetMwFifthCharacterNotes(string characterId, string? notes) =>
        await _characterManager.SetNotes(characterId, notes);

    public async Task SetAccountPreregistrationNotes(string accountId, string? notes)
    {
        await _userSessionManager.UpdateUserAccount(accountId, update => update
            .Set(x => x.MwFifthPreregistrationNotes, notes));
    }

    public async Task SetAccountAdminNotes(string accountId, string? notes)
    {
        await _userSessionManager.UpdateUserAccount(accountId, update => update
            .Set(x => x.AdminNotes, notes));
    }

    public async Task<PreregistrationNotes> GetEventNotes(string eventId)
    {
        var info = await _letterManager.GetByEvent(eventId);
        var ev = info.Events.Values.FirstOrDefault()
                 ?? throw new ResourceNotFoundException();

        var gameState = await _db.MwFifthGame.GetGameState();
        ev.Chapter ??= gameState.HomeChapters.FirstOrDefault(x =>
            ev.Title!.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase))?.Name;

        var attendance = (await _db.Attendances.Find(x => x.EventId == eventId).ToListAsync());
        var accountIds = info.Letters.Select(x => x.Value.AccountId)
            .Concat(attendance.Select(x => x.AccountId))
            .Distinct().ToList();
        var accounts = (await _db.Accounts
                .Find(x => accountIds.Contains(x.AccountId))
                .ToListAsync())
            .ToDictionary(x => x.AccountId);
        var accountCharacters = (await _db.MwFifthGame.Characters
                .Find(x => x.State == CharacterState.Live && accountIds.Contains(x.AccountId))
                .ToListAsync())
            .ToLookup(x => x.AccountId);
        var accountRevisions = (await _db.MwFifthGame.CharacterRevisions
                .Find(x => x.State == CharacterState.Live && accountIds.Contains(x.AccountId))
                .ToListAsync())
            .ToDictionary(x => x.CharacterId);

        foreach (var attendee in attendance)
        {
            if (info.Letters.Any(x => x.Value.AccountId == attendee.AccountId)) continue;

            var id = LarpContext.GenerateNewId();
            info.Letters.Add(id, new Letter
            {
                LetterId = id,
                AccountId = attendee.AccountId,
                State = LetterState.Approved,
                Name = LetterNames.PreEvent,
                TemplateId = ev.LetterTemplates.FirstOrDefault(x => x.Name == LetterNames.PreEvent)?.LetterTemplateId ??
                             "",
                Fields =
                {
                    { "pcing", string.Join(",", ev.Components.Select(x => x.Name)) },
                    { "waiver", "true" }
                }
            });
        }

        var attendees = new List<PlayerAttendee>();
        foreach (var (_, letter) in info.Letters)
        {
            if (letter.Name != LetterNames.PreEvent) continue;
            var account = accounts.GetValueOrDefault(letter.AccountId);
            var characters = accountCharacters[letter.AccountId].ToList();
            var attendee = new PlayerAttendee
            {
                AccountId = account?.AccountId,
                Name = account?.Name ?? "No Name Set",
                Age = account?.Age?.ToString(),
                Notes = account?.Notes,
                PreEventLetter = letter,
                DiscountPercentage = account?.DiscountPercent,
                Characters = (
                    from character in characters
                    let revision = accountRevisions[character.CharacterId]
                    select new CharacterAttendee()
                    {
                        CharacterId = character.CharacterId,
                        RevisionId = revision.RevisionId,
                        Name = character.CharacterName ?? "No Name Set",
                        HomeChapter = revision.HomeChapter ?? "Homeless",
                        Notes = character.PreregistrationNotes,
                        GeneratedNotes = revision.PreregistrationNotes,
                        Skills = revision.ConsolidatedSkills(),
                        Advantages = revision.Advantages.Select(x => new NameRank(x.Name, x.Rank)).ToArray(),
                        Disadvantages = revision.Disadvantages.Select(x => new NameRank(x.Name, x.Rank)).ToArray()
                    }).ToArray()
            };
            attendees.Add(attendee);
        }

        return new PreregistrationNotes()
        {
            Event = ev,
            Attendees = attendees.ToArray()
        };
    }

    public async Task UninviteAccount(string accountId)
    {
        await Log(new AccountStateLogEvent { AccountId = accountId, AccountState = AccountState.Uninvited });
        await _userSessionManager.UpdateUserAccount(accountId, x => x
            .Set(account => account.State, AccountState.Uninvited)
        );
    }

    public async Task ArchiveAccount(string accountId)
    {
        await Log(new AccountStateLogEvent { AccountId = accountId, AccountState = AccountState.Archived });
        await _userSessionManager.UpdateUserAccount(accountId, x => x
            .Set(account => account.State, AccountState.Archived)
            .Set(account => account.Emails, new List<AccountEmail>())
        );
    }

    public async Task RestoreAccount(string accountId)
    {
        await Log(new AccountStateLogEvent { AccountId = accountId, AccountState = AccountState.Active });
        await _userSessionManager.UpdateUserAccount(accountId, x => x
            .Set(account => account.State, AccountState.Active)
        );
    }

    public async Task<string[]> GetLog()
    {
        var names = await GetAccountNames();
        var logEntries = await _db.EventLog.Find(_ => true).ToListAsync();
        var sb = new StringBuilder();
        return logEntries.Select(log =>
        {
            sb.Clear()
                .Append("[")
                .Append(DateTimeOffset.Parse(log[nameof(LogEvent.ActedOn)].AsString).ToString("MMM d, yyyy h:mm tt"))
                .Append("] ")
                .Append(log[nameof(LogEvent.Type)].AsString)
                .Append(" (")
                .Append(names.GetValueOrDefault(log[nameof(LogEvent.ActorAccountId)].AsObjectId.ToString()!)?.Name ??
                        "Unknown Account")
                .Append(") ");

            log.Remove("_id");
            log.Remove(nameof(LogEvent.LogEventId));
            log.Remove(nameof(LogEvent.ActorAccountId));
            log.Remove(nameof(LogEvent.ActorSessionId));
            log.Remove(nameof(LogEvent.ActedOn));
            log.Remove(nameof(LogEvent.Type));
            foreach (var element in log.Elements.ToList())
            {
                if (element.Name.EndsWith("AccountId") && element.Value.IsObjectId)
                {
                    var account = names.GetValueOrDefault(element.Value.AsObjectId.ToString()!);
                    if (account == null) log[element.Name] = "Invalid Account";
                    else log[element.Name] = account.Name ?? "No Name Set";
                }
            }

            sb.Append(log.ToJson());
            return sb.ToString();
        }).ToArray();
    }

    public async Task ReseedData()
    {
        await _backupManager.Reseed();
        await Log(new GameStateLogEvent() { GameName = GameState.GameName, Summary = $"Reseed" });
    }

    public async Task DeleteCharactersUnused()
    {
        var characters = await _db.MwFifthGame.Characters
            .Find(c => c.ImportId.HasValue)
            .ToListAsync();

        var revisions = (await _db.MwFifthGame.CharacterRevisions.AsQueryable()
                .GroupBy(x => x.CharacterId)
                .Select(x => new
                {
                    CharacterId = x.Key,
                    Count = x.Count()
                }).ToListAsync())
            .ToDictionary(x => x.CharacterId, x => x.Count);

        var deletes = new List<string>();
        deletes.AddRange(characters.Where(x => revisions[x.CharacterId] == 1).Select(x => x.CharacterId));
        await _db.MwFifthGame.Characters.DeleteManyAsync(x => deletes.Contains(x.CharacterId));
        await _db.MwFifthGame.CharacterRevisions.DeleteManyAsync(x => deletes.Contains(x.CharacterId));
    }

    public async Task SaveOccupations(Occupation[] occupations)
    {
        foreach (var occupation in occupations)
        {
            if (occupation.Specialties?.Length == 0)
                occupation.Specialties = null;
            if (occupation.Choices?.Length == 0)
                occupation.Choices = null;
            if (occupation.Chapters?.Length == 0)
                occupation.Chapters = null;
        }

        await UpdateGameState(nameof(GameState.Occupations), occupations);
    }

    public async Task SaveSpells(Spell[] spells)
    {
        foreach (var spell in spells)
        {
            if (spell.Categories.Length == 0 || spell.Categories.Any(string.IsNullOrWhiteSpace))
                spell.Categories = new[] { "Gift of Wisdom" };
        }

        await UpdateGameState(nameof(GameState.Spells), spells);
    }

    public async Task SaveSkills(SkillDefinition[] skills)
    {
        foreach (var skill in skills)
        {
            if (skill.Chapters?.Length == 0)
                skill.Chapters = null;
            if (skill.CostPerPurchase == 0)
                skill.CostPerPurchase = null;
            if (skill.RanksPerPurchase == 0)
                skill.RanksPerPurchase = null;
            if (string.IsNullOrWhiteSpace(skill.Description))
                skill.Description = "";
            // if (!skill.Title.StartsWith(skill.Name))
            //     throw new BadRequestException($"Skill '{skill.Name}' with title '{skill.Title}' do not match");
            if (skill is { Purchasable: SkillPurchasable.Multiple or SkillPurchasable.Once, CostPerPurchase: null })
                throw new BadRequestException(
                    $"Skill '{skill.Name}' requires {nameof(SkillDefinition.CostPerPurchase)}");
            if (skill is { Purchasable: SkillPurchasable.Multiple, RanksPerPurchase: null })
                skill.RanksPerPurchase = 1;
        }

        await UpdateGameState(nameof(GameState.Skills), skills);
    }

    public async Task SaveAdvantages(Vantage[] vantages)
    {
        foreach (var vantage in vantages)
            vantage.Title = $"{vantage.Name} {vantage.Rank}";

        await UpdateGameState(nameof(GameState.Advantages), vantages);
    }

    public async Task SaveDisadvantages(Vantage[] vantages)
    {
        foreach (var vantage in vantages)
            vantage.Title = $"{vantage.Name} {vantage.Rank}";

        await UpdateGameState(nameof(GameState.Disadvantages), vantages);
    }

    public async Task SaveChapters(HomeChapter[] chapters)
    {
        foreach (var chapter in chapters)
            chapter.Name = NormalizeName(chapter.Name, chapter.Title);

        await UpdateGameState(nameof(GameState.HomeChapters), chapters);
    }

    public async Task SaveReligions(Religion[] religions)
    {
        foreach (var religion in religions)
            religion.Name = NormalizeName(religion.Name, religion.Title);

        await UpdateGameState(nameof(GameState.Religions), religions);
    }

    public async Task SetTerm(string name, string? summary, string? description)
    {
        var gameId = await _db.GetGameIdByName(GameState.GameName);

        await _db.ClarifyTerms.UpdateOneAsync(
            term => term.Name == name && term.GameId == gameId,
            Builders<ClarifyTerm>.Update
                .SetOnInsert(term => term.Name, name)
                .SetOnInsert(term => term.GameId, gameId)
                .Set(term => term.Summary, summary)
                .Set(term => term.Description, description),
            new UpdateOptions { IsUpsert = true });
    }

    public async Task DeleteTerm(string name)
    {
        var gameId = await _db.GetGameIdByName(GameState.GameName);

        await _db.ClarifyTerms.DeleteOneAsync(term => term.Name == name && term.GameId == gameId);
    }

    public async Task<Citation[]> GetCitations(string accountId)
    {
        return await _db.Citations.Find(citation => citation.AccountId == accountId).ToArrayAsync();
    }

    public async Task UpdateCitations(Citation citation)
    {
        await _citationManager.Save(citation, _account.AccountId);
    }

    public async Task SetCitationState(string citationId, CitationState state)
    {
        await _citationManager.SetState(citationId, state, _account.AccountId);
    }

    public async Task<EventAttendance[]> GetAccountAttendances(string accountId) =>
        await _eventManager.GetAccountAttendances(accountId);

    public async Task<EventAndLetters[]> GetEvents() =>
        await _eventManager.GetEvents();

    public async Task<Event> GetEvent(string eventId) =>
        await _eventManager.GetEvent(eventId);

    public async Task SaveEvent(string eventId, Event @event)
    {
        var oldEvent = await _eventManager.GetEvent(eventId);
        var changeSummary = Event.BuildChangeSummary(oldEvent, @event);

        await Log(new EventChangeLogEvent
        {
            EventId = eventId, Summary = "Update",
            ChangeSummary = JsonSerializer.Serialize(changeSummary, new JsonSerializerOptions { WriteIndented = true })
        });
        await _eventManager.SaveEvent(eventId, @event);
    }

    public async Task DeleteEvent(string eventId)
    {
        await Log(new EventChangeLogEvent { EventId = eventId, Summary = "Delete" });
        await _eventManager.DeleteEvent(eventId);
    }

    public async Task SetEventAttendance(
        string eventId, string accountId, bool attended,
        int? moonstone1, int? moonstone2,
        decimal? paid, decimal? expected, string[] characterIds) =>
        await _eventManager.SetEventAttendance(eventId, accountId, attended, moonstone1, moonstone2, paid, expected,
            characterIds);

    public async Task<Attendance[]> GetEventAttendances(string eventId) =>
        await _eventManager.GetEventAttendances(eventId);

    public Task<StringResult> Import(Stream data) =>
        _backupManager.Import(data);

    public Task<IFileInfo> Export() =>
        _backupManager.Export();

    public async Task<Dictionary<string, AccountName>> GetAccountNames()
    {
        var names = await _db.Accounts.Find(_ => true)
            .Project(account => new AccountName
            {
                AccountId = account.AccountId,
                State = account.State,
                Name = account.Name,
                Emails = account.Emails
            })
            .ToListAsync();
        return names.ToDictionary(x => x.AccountId);
    }

    public async Task<StringResult> AddAdminAccount(string fullName, string emailAddress)
    {
        await Log(new AddAdminLogEvent { FullName = fullName, EmailAddress = emailAddress });
        return StringResult.Success(await _userSessionManager.AddAdminAccount(fullName, emailAddress));
    }

    public async Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state) =>
        await _characterManager.GetState(state);

    public async Task<CharacterAndRevision> GetMwFifthCharacter(string characterId) =>
        await _characterManager.GetRevision(characterId, _account, true)
        ?? throw new ResourceNotFoundException();

    public async Task<Dashboard> GetDashboard()
    {
        return new Dashboard
        {
            Accounts =
                (int)await _db.Accounts.CountDocumentsAsync(_ => true),
            VerifiedAccounts =
                (int)await _db.Accounts.CountDocumentsAsync(
                    Builders<Account>.Filter.ElemMatch(x => x.Emails, x => x.IsVerified)),
            MwFifthCharacters =
                (int)await _db.MwFifthGame.CharacterRevisions.CountDocumentsAsync(x => x.State == CharacterState.Live),
            MwFifthReview =
                (int)await _db.MwFifthGame.CharacterRevisions.CountDocumentsAsync(x =>
                    x.State == CharacterState.Review),
            ReviewLetters =
                (int)await _db.Letters.CountDocumentsAsync(x => x.State == LetterState.Submitted),
        };
    }

    public async Task RemoveAccountRole(string accountId, AccountRole role)
    {
        await Log(new AccountRoleLogEvent { AccountId = accountId, RemoveRole = role });
        await _userSessionManager.RemoveAccountRole(accountId, role);
    }

    public async Task AddAccountRole(string accountId, AccountRole role)
    {
        await Log(new AccountRoleLogEvent { AccountId = accountId, AddRole = role });
        await _userSessionManager.AddAccountRole(accountId, role);
    }

    public async Task<Account> GetAccount(string accountId)
    {
        return await _db.Accounts.Find(x => x.AccountId == accountId).FirstOrDefaultAsync();
    }

    public async Task<CharacterSummary[]> GetAccountCharacters(string accountId)
    {
        var gameState = await _db.MwFifthGame.GetGameState();

        var list = await _db.MwFifthGame.CharacterRevisions.Find(x =>
                x.AccountId == accountId && (x.State == CharacterState.Live || x.State == CharacterState.Review))
            .ToListAsync();

        return list.Select(x => x.ToSummary(gameState)).ToArray();
    }

    public async Task UpdateAccount(string accountId, string? name, string? location, string? phone,
        DateOnly? birthDate, string? notes, int? discount)
    {
        await _userSessionManager.UpdateUserAccount(accountId, update => update
            .Set(x => x.Name, name)
            .Set(x => x.Location, location)
            .Set(x => x.Phone, phone)
            .Set(x => x.NormalizedPhone, Account.BuildNormalizedPhone(phone))
            .Set(x => x.Notes, notes)
            .Set(x => x.BirthDate, birthDate)
            .Set(x => x.DiscountPercent, discount));
    }

    public async Task<CharacterAndRevision> GetMwFifthCharacterLatest(string characterId) =>
        await _characterManager.GetLatest(characterId, _account, true);

    public async Task<CharacterAndRevisions> GetMwFifthCharacterRevisions(string characterId) =>
        await _characterManager.GetRevisions(characterId, _account, true);

    private static string NormalizeName(string religionName, string religionTitle)
    {
        var name = string.IsNullOrWhiteSpace(religionName) ? religionTitle : religionName;
        return Regex.Replace(name.ToLowerInvariant(), "[^a-z]", "");
    }

    private async Task UpdateGameState<TField>(string property, TField items)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(nameof(GameState.Name), GameState.GameName);
        var update = Builders<BsonDocument>.Update
            .Set(property, items)
            .Set(nameof(GameState.Revision), Guid.NewGuid().ToString("N"))
            .Set(nameof(GameState.LastUpdated), _clock.UtcNow.ToString("O"));
        var result = await _db.GameStates.UpdateOneAsync(filter, update);

        if (result.ModifiedCount != 1)
            throw new BadRequestException("Game State could not be updated");

        _db.MwFifthGame.ClearGameState();

        await Log(new GameStateLogEvent() { GameName = GameState.GameName, Summary = $"Updated {property}" });
    }

    private async Task Log<TLogEvent>(TLogEvent logEvent) where TLogEvent : LogEvent
    {
        await _db.LogEvent(_account.AccountId, _sessionId, logEvent);
    }
}