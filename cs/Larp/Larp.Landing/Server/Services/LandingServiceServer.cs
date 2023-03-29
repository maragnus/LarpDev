using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Larp.Notify;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Larp.Landing.Server.Services;

public class LandingServiceServer : ILandingService
{
    private readonly LarpContext _db;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IUserSession _userSession;
    private readonly INotifyService _notifyService;
    private readonly IFileProvider _fileProvider;

    public LandingServiceServer(LarpContext db, IUserSessionManager userSessionManager, IUserSession userSession, INotifyService notifyService, IFileProvider fileProvider)
    {
        _db = db;
        _userSessionManager = userSessionManager;
        _userSession = userSession;
        _notifyService = notifyService;
        _fileProvider = fileProvider;
    }

    public async Task<Result> Login(string email, string deviceName)
    {
        var token = await _userSessionManager.GenerateToken(email, deviceName);

        await _notifyService.SendEmailAsync(email, "LARP Landing", @$"Your sign in code for LARP Landing is {token}");
    
        return Result.Success;
    }
    
    public async Task<Result> Logout()
    {
        if (_userSession.SessionId == null)
            return Result.Failed("You are not logged in");
        await _userSessionManager.DestroyUserSession(_userSession.SessionId);
        return Result.Success;
    }

    public async Task<StringResult> Confirm(string email, string token, string deviceName)
    {
        var sessionId = await _userSessionManager.CreateUserSession(email, token, deviceName);
        return StringResult.Success(sessionId);
    }

    public async Task<Result> Validate()
    {
        var result = await _userSessionManager.ValidateUserSession(_userSession.SessionId);
        switch (result.StatusCode)
        {
            case UserSessionValidationResultStatusCode.Authenticated: 
                return Result.Success;
            case UserSessionValidationResultStatusCode.Invalid:
            case UserSessionValidationResultStatusCode.NotConfirmed:
            case UserSessionValidationResultStatusCode.Expired:
            default:
                return Result.Failed(result.StatusCode.ToString());
        }
    }

    public async Task<Game[]> GetGames()
    {
        var games = await _db.Games.Find(x => true).ToListAsync();
        return games.ToArray();
    }

    public async Task<CharacterSummary[]> GetCharacters()
    {
        var gameState = await _db.MwFifthGame.GetGameState();
        var characters = await _db.MwFifthGame.Characters.Find(x => x.State != CharacterState.Archived).ToListAsync();
        return characters.Select(x => x.ToSummary(gameState)).ToArray();
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
                OccupationalSkills = SkillList(p.Skills.Where(x=>x.Type is SkillPurchase.Occupation or SkillPurchase.OccupationChoice)),
                PurchasedSkills = SkillList(p.Skills.Where(x=>x.Type is SkillPurchase.Purchased)),
                PurchasedSkillMoonstone = 0,
                FreeSkills = SkillList(p.Skills.Where(x=>x.Type is SkillPurchase.Free or SkillPurchase.Bestowed)),
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
}