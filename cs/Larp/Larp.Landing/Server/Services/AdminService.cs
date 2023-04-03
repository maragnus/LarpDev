using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Microsoft.Extensions.FileProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using MwFifthCharacter = Larp.Data.MwFifth.Character;

namespace Larp.Landing.Server.Services;

public class AdminService : IAdminService
{
    private readonly LarpContext _db;
    private readonly IFileProvider _fileProvider;
    private readonly MwFifthCharacterManager _manager;
    private readonly IUserSessionManager _userSessionManager;
    private readonly Account _account;

    public AdminService(LarpContext db, IUserSession userSession, IFileProvider fileProvider,
        MwFifthCharacterManager manager, IUserSessionManager userSessionManager)
    {
        _db = db;
        _fileProvider = fileProvider;
        _manager = manager;
        _userSessionManager = userSessionManager;
        _account = userSession.CurrentUser!;
    }

    public async Task<Account[]> GetAccounts()
    {
        var accounts = await _db.Accounts
            .Find(_ => true)
            .ToListAsync();
        return accounts.ToArray();
    }

    public async Task<IFileInfo> Export()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var path = _fileProvider.GetFileInfo($"{Path.GetRandomFileName()}.xlsx");

        var file = new FileInfo(path.PhysicalPath!);
        using var package = new ExcelPackage(file);

        using var workbook = package.Workbook;

        var playerSheet = workbook.Worksheets.Add("Players");
        {
            var players = await _db.Accounts
                .Find(_ => true)
                .ToListAsync();
            playerSheet.Cells.LoadFromCollection(players.Select(p => new
            {
                PlayerId = p.AccountId,
                PlayerName = p.Name,
                p.Phone,
                p.Emails.FirstOrDefault(x => x.IsPreferred)?.Email,
                p.Location,
                p.Notes,
            }), true, TableStyles.Light6);
        }

        var characterSheet = workbook.Worksheets.Add("Characters");
        {
            var players = await _db.MwFifthGame.Characters
                .Find(x => x.State == CharacterState.Live)
                .ToListAsync();
            characterSheet.Cells.LoadFromCollection(players.Select(p => new
            {
                CharacterId = p.Id,
                p.AccountId,
                TotalMoonstone = 0,
                UnspentMoonstone = 0,
                p.CharacterName,
                p.HomeChapter,
                p.Occupation,
                p.Specialty,
                OccupationalEnhancement = p.Enhancement,
                p.Homeland,
                p.Religion,
                p.Courage,
                p.Dexterity,
                p.Empathy,
                p.Passion,
                p.Prowess,
                p.Wisdom,
                p.Level,
                OccupationalSkills = SkillList(p.Skills.Where(x =>
                    x.Type is SkillPurchase.Occupation or SkillPurchase.OccupationChoice)),
                PurchasedSkills = SkillList(p.Skills.Where(x => x.Type is SkillPurchase.Purchased)),
                PurchasedSkillMoonstone = 0,
                FreeSkills = SkillList(p.Skills.Where(x => x.Type is SkillPurchase.Free or SkillPurchase.Bestowed)),
                Advantages = VantageList(p.Advantages),
                Disadvantages = VantageList(p.Disadvantages),
                Spells = string.Join(", ", p.Spells),
                p.Cures,
                p.Documents,
                p.PublicHistory,
                p.PrivateHistory,
                p.UnusualFeatures,
                p.Notes
            }), true, TableStyles.Light6);
        }

        string SkillList(IEnumerable<CharacterSkill> skills) =>
            string.Join(", ", skills.Select(x => x.Title));

        string VantageList(IEnumerable<CharacterVantage> vantages) =>
            string.Join(", ", vantages.Select(x => x.Title));

        await package.SaveAsync();

        return path;
    }

    public async Task ApproveMwFifthCharacter(string characterId) =>
        await _manager.Approve(characterId);

    public async Task RejectMwFifthCharacter(string characterId) =>
        await _manager.Reject(characterId);

    public async Task<Character> ReviseMwFifthCharacter(string characterId) =>
        await _manager.GetDraft(characterId, _account, true);

    public async Task SaveMwFifthCharacter(Character character) =>
        await _manager.Save(character, _account, true);

    public async Task DeleteMwFifthCharacter(string characterId) =>
        await _manager.Delete(characterId, _account, true);

    public async Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state)
    {
        var list = await _db.MwFifthGame.Characters.AsQueryable()
            .Where(character => character.State == state)
            .Join(_db.Accounts.AsQueryable(),
                character => character.AccountId,
                account => account.AccountId,
                (character, account) => new { Character = character, Account = account })
            .ToListAsync();

        return list
            .Select(x => new CharacterAccountSummary(x.Character, x.Account))
            .ToArray();
    }

    public async Task<MwFifthCharacter> GetMwFifthCharacter(string characterId) =>
        await _manager.Get(characterId, _account, true)
        ?? throw new ResourceNotFoundException();

    public async Task<Dashboard> GetDashboard()
    {
        return new Dashboard
        {
            Accounts = (int)await _db.Accounts.CountDocumentsAsync(_ => true),
            MwFifthCharacters =
                (int)await _db.MwFifthGame.Characters.CountDocumentsAsync(x => x.State == CharacterState.Live),
            MwFifthReview =
                (int)await _db.MwFifthGame.Characters.CountDocumentsAsync(x => x.State == CharacterState.Review)
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

        var list = await _db.MwFifthGame.Characters.Find(x =>
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

    public async Task<MwFifthCharacter> GetMwFifthCharacterLatest(string characterId) =>
        await _manager.GetLatest(characterId, _account, true);

    public async Task<MwFifthCharacter[]> GetMwFifthCharacterRevisions(string characterId) =>
        await _manager.GetRevisions(characterId, _account, true);
}