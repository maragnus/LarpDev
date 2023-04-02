using Larp.Data;
using Larp.Data.Mongo;
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
    private readonly IUserSession _userSession;
    private readonly IFileProvider _fileProvider;

    public AdminService(LarpContext db, IUserSession userSession, IFileProvider fileProvider)
    {
        _db = db;
        _userSession = userSession;
        _fileProvider = fileProvider;
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

    public async Task ApproveMwFifthCharacter(string characterId)
    {
        var character = await GetMwFifthCharacter(characterId);
        if (character.State != CharacterState.Review)
            throw new BadRequestException("Character is not in review state");

        // Mark all (hopefully only one) Live characters are Archive
        await _db.MwFifthGame.Characters
            .UpdateOneAsync(x => x.UniqueId == character.UniqueId && x.State == CharacterState.Live,
                Builders<MwFifthCharacter>.Update
                    .Set(x => x.State, CharacterState.Archived)
                    .Set(x => x.ArchivedOn, DateTime.UtcNow));

        await _db.MwFifthGame.Characters
            .UpdateOneAsync(x => x.Id == characterId,
                Builders<MwFifthCharacter>.Update
                    .Set(x => x.State, CharacterState.Live)
                    .Set(x => x.ApprovedOn, DateTime.UtcNow));
    }

    public async Task RejectMwFifthCharacter(string characterId)
    {
        var character = await GetMwFifthCharacter(characterId);
        if (character.State != CharacterState.Review)
            throw new BadRequestException("Character is not in review state");

        var state = character.PreviousId == null
            ? CharacterState.NewDraft
            : CharacterState.UpdateDraft;

        var update = Builders<MwFifthCharacter>.Update
            .Set(x => x.State, state);

        await _db.MwFifthGame.Characters
            .UpdateOneAsync(x => x.Id == characterId, update);
    }

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

    public async Task<Character> ReviseMwFifthCharacter(string characterId)
    {
        var character = await GetMwFifthCharacterLatest(characterId);

        if (character.State is CharacterState.NewDraft or CharacterState.UpdateDraft)
            return character;

        if (character.State != CharacterState.Live)
            throw new BadRequestException("Character must be Live to revise");

        character.PreviousId = character.Id;
        character.Id = ObjectId.GenerateNewId().ToString();
        character.State = CharacterState.UpdateDraft;
        character.UniqueId = character.UniqueId;
        character.CreatedOn = character.CreatedOn;

        await _db.MwFifthGame.Characters.InsertOneAsync(character);
        return character;
    }

    public Task SaveMwFifthCharacter(Character character)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAccountRole(string accountId, AccountRole role)
    {
        var account = await _db.Accounts.FindOneAsync(x=>x.AccountId == accountId)
                      ?? throw new ResourceNotFoundException();

        var roles = account.Roles.ToHashSet();
        if (!roles.Remove(role)) return;

        await _db.Accounts.UpdateByIdAsync(x=>x.AccountId == accountId,
            x =>
                x.Set(a => a.Roles, roles.ToArray()));
    }

    public async Task AddAccountRole(string accountId, AccountRole role)
    {
        var account = await _db.Accounts.FindOneAsync(x=>x.AccountId == accountId)
                      ?? throw new ResourceNotFoundException();
        var roles = account.Roles.ToHashSet();
        if (!roles.Add(role)) return;

        await _db.Accounts.UpdateByIdAsync(x=>x.AccountId == accountId,
            x =>
                x.Set(a => a.Roles, roles.ToArray()));
    }

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

    public async Task<MwFifthCharacter> GetMwFifthCharacter(string characterId)
    {
        return await _db.MwFifthGame.Characters.Find(x => x.Id == characterId).FirstOrDefaultAsync();
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

    public async Task<MwFifthCharacter> GetMwFifthCharacterLatest(string characterId)
    {
        var character = await GetMwFifthCharacter(characterId)
                        ?? throw new Exception("Character not found");

        var revisions = await _db.MwFifthGame.Characters
            .Find(x => x.UniqueId == character.UniqueId && x.State != CharacterState.Archived)
            .ToListAsync();

        return revisions.FirstOrDefault(x => x.State is CharacterState.NewDraft or CharacterState.UpdateDraft)
               ?? revisions.FirstOrDefault(x => x.State == CharacterState.Review)
               ?? revisions.FirstOrDefault(x => x.State == CharacterState.Live)
               ?? character;
    }

    public async Task<MwFifthCharacter[]> GetMwFifthCharacterRevisions(string characterId)
    {
        var character = await GetMwFifthCharacter(characterId)
                             ?? throw new Exception("Character not found");

        var revisions = await _db.MwFifthGame.Characters
            .Find(x => x.UniqueId == character.UniqueId)
            .ToListAsync();

        return revisions
            .OrderBy(x => x.ApprovedOn ?? x.SubmittedOn ?? x.ArchivedOn ?? x.CreatedOn)
            .ToArray();
    }
}