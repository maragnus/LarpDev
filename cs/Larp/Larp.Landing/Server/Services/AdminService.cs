using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Microsoft.Extensions.FileProviders;
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

        if (character.PreviousId != null)
        {
            await _db.MwFifthGame.Characters
                .UpdateOneAsync(x => x.Id == character.PreviousId, 
                    Builders<MwFifthCharacter>.Update
                        .Set(x => x.State, CharacterState.Archived)
                        .Set(x => x.ArchivedOn, DateTime.UtcNow));
        }

        await _db.MwFifthGame.Characters
            .UpdateOneAsync(x => x.Id == characterId, 
                Builders<MwFifthCharacter>.Update
                .Set(x => x.State, CharacterState.Live)
                .Set(x => x.ApprovedOn, DateTime.UtcNow));
    }

    public async Task RejectMwFifthCharacter(string characterId)
    {
        var character = await GetMwFifthCharacter(characterId);

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
        var dashboard = new Dashboard();
        
        dashboard.Accounts = (int)await _db.Accounts.CountDocumentsAsync( _=> true);
        dashboard.MwFifthCharacters = (int)await _db.MwFifthGame.Characters.CountDocumentsAsync(x => x.State == CharacterState.Live);
        dashboard.MwFifthReview = (int)await _db.MwFifthGame.Characters.CountDocumentsAsync(x => x.State == CharacterState.Review);

        return dashboard;
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
        var list = await _db.MwFifthGame.Characters.Find(x => x.AccountId == accountId && (x.State == CharacterState.Live || x.State == CharacterState.Review)).ToListAsync();
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
        // var subject = _db.MwFifthGame.Characters;
        // var connectFromField = (FieldDefinition<MwFifthCharacter, string>)"PreviousId";
        // var connectToField = (FieldDefinition<MwFifthCharacter, string>)"_id";
        // var startWith = (AggregateExpressionDefinition<MwFifthCharacter, string>)"$reportsTo";
        // var @as = (FieldDefinition<MwFifthCharacterHierarchy, IEnumerable<MwFifthCharacter>>)"ReportingHierarchy";
        //
        //
        // // TODO -- replace with graph lookup
        // var list = await subject
        //     .Aggregate()
        //     .Match(x => x.Id == characterId)
        //     .GraphLookup(subject, connectFromField, connectToField, startWith, @as)
        //     .ToListAsync();

        var character = await GetMwFifthCharacter(characterId)
                        ?? throw new Exception("Character not found");

        while (true)
        {
            var id = character.Id;
            var nextCharacter = await _db.MwFifthGame.Characters
                .Find(x => x.PreviousId == id)
                .FirstOrDefaultAsync();
            if (nextCharacter == null)
                return character;
            character = nextCharacter;
        }
    }

    public async Task<MwFifthCharacter[]> GetMwFifthCharacterRevisions(string characterId)
    {
        var characters = new LinkedList<MwFifthCharacter>();

        var firstCharacter = await GetMwFifthCharacter(characterId)
                             ?? throw new Exception("Character not found");
        characters.AddFirst(firstCharacter);
        var character = firstCharacter;

        while (true)
        {
            var id = character.Id;
            var nextCharacter = await _db.MwFifthGame.Characters
                .Find(x => x.PreviousId == id)
                .FirstOrDefaultAsync();
            if (nextCharacter == null)
                break;
            character = nextCharacter;
            characters.AddLast(character);
        }

        character = firstCharacter;
        while (true)
        {
            var nextCharacter = await _db.MwFifthGame.Characters
                .Find(x => x.Id == character.PreviousId)
                .FirstOrDefaultAsync();
            if (nextCharacter == null)
                break;
            character = nextCharacter;
            characters.AddFirst(character);
        }

        return characters.ToArray();
    }
}