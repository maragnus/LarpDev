using Larp.Common;
using Larp.Common.Exceptions;
using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class AdminService : IAdminService
{
    private readonly LarpContext _db;
    private readonly MwFifthCharacterManager _characterManager;
    private readonly IUserSessionManager _userSessionManager;
    private readonly LetterManager _letterManager;
    private readonly Account _account;
    private readonly EventManager _eventManager;
    private readonly AttachmentManager _attachmentManager;
    private readonly BackupManager _backupManager;

    public AdminService(LarpContext db, IUserSession userSession,
        MwFifthCharacterManager characterManager, IUserSessionManager userSessionManager,
        LetterManager letterManager, EventManager eventManager,
        AttachmentManager attachmentManager, BackupManager backupManager)
    {
        _db = db;
        _characterManager = characterManager;
        _userSessionManager = userSessionManager;
        _letterManager = letterManager;
        _eventManager = eventManager;
        _attachmentManager = attachmentManager;
        _backupManager = backupManager;
        _account = userSession.CurrentUser!;
    }

    public async Task<Account[]> GetAccounts()
    {
        var accounts = await _db.Accounts
            .Find(_ => true)
            .ToListAsync();
        return accounts.ToArray();
    }

    public Task<IFileInfo> ExportLetters(string eventId) =>
        _backupManager.ExportLetters(eventId);

    async Task IAdminService.MergeAccounts(string fromAccountId, string toAccountId)
    {
        await _characterManager.MoveAll(fromAccountId, toAccountId);
        var attendances = await _db.Attendances
            .Find(attendance => attendance.AccountId == fromAccountId).ToListAsync();

        foreach (var attendance in attendances)
        {
            await SetEventAttendance(attendance.EventId, toAccountId, true,
                attendance.MwFifth?.Moonstone,
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
            $"{fromAccount.Notes ?? ""} {toAccount.Notes ?? ""}".Trim()
        );

        await _db.Accounts.DeleteOneAsync(account => account.AccountId == fromAccountId);

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

    public async Task RejectMwFifthCharacter(string characterId) =>
        await _characterManager.Reject(characterId);

    public async Task<CharacterAndRevision> ReviseMwFifthCharacter(string characterId) =>
        await _characterManager.GetDraft(characterId, _account, true);

    public async Task SaveMwFifthCharacter(CharacterRevision revision) =>
        await _characterManager.Save(revision, _account, true);

    public async Task DeleteMwFifthCharacter(string characterId) =>
        await _characterManager.Delete(characterId, _account, true);

    public async Task MoveMwFifthCharacter(string characterId, string newAccountId) =>
        await _characterManager.Move(characterId, newAccountId);

    public async Task<StringResult> DraftEvent() => await _eventManager.DraftEvent();

    public async Task<EventAndLetters[]> GetEvents() =>
        await _eventManager.GetEvents();

    public async Task<Event> GetEvent(string eventId) =>
        await _eventManager.GetEvent(eventId);

    public async Task SaveEvent(string eventId, Event @event) =>
        await _eventManager.SaveEvent(eventId, @event);

    public async Task DeleteEvent(string eventId) =>
        await _eventManager.DeleteEvent(eventId);

    public async Task SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone,
        string[] characterIds) =>
        await _eventManager.SetEventAttendance(eventId, accountId, attended, moonstone, characterIds);

    public async Task<Attendance[]> GetEventAttendances(string eventId) =>
        await _eventManager.GetEventAttendances(eventId);

    public Task<StringResult> Import(Stream data) =>
        _backupManager.Import(data);

    public Task<IFileInfo> Export() =>
        _backupManager.Export();

    public async Task<Dictionary<string, AccountName>> GetAccountNames()
    {
        var names = await _db.Accounts.Find(_ => true)
            .Project(account => new AccountName()
            {
                AccountId = account.AccountId,
                Name = account.Name,
                Emails = account.Emails
            })
            .ToListAsync();
        return names.ToDictionary(x => x.AccountId);
    }

    public async Task<StringResult> AddAdminAccount(string fullName, string emailAddress) =>
        StringResult.Success(await _userSessionManager.AddAdminAccount(fullName, emailAddress));

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
        await _userSessionManager.RemoveAccountRole(accountId, role);
    }

    public async Task AddAccountRole(string accountId, AccountRole role)
    {
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
        DateOnly? birthDate, string? notes)
    {
        var update = Builders<Account>.Update
            .Set(x => x.Name, name)
            .Set(x => x.Location, location)
            .Set(x => x.Phone, phone)
            .Set(x => x.Notes, notes)
            .Set(x => x.BirthDate, birthDate);

        await _db.Accounts.UpdateOneAsync(x => x.AccountId == accountId, update);
    }

    public async Task<CharacterAndRevision> GetMwFifthCharacterLatest(string characterId) =>
        await _characterManager.GetLatest(characterId, _account, true);

    public async Task<CharacterAndRevisions> GetMwFifthCharacterRevisions(string characterId) =>
        await _characterManager.GetRevisions(characterId, _account, true);
}