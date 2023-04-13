using Larp.Common;
using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Larp.Landing.Server.Import;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
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

        var playerSheet = workbook.Worksheets.Add("Players");
        {
            var players = await _larpContext.Accounts
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
            var players = await _larpContext.MwFifthGame.CharacterRevisions
                .Find(x => x.State == CharacterState.Live)
                .ToListAsync();
            characterSheet.Cells.LoadFromCollection(players.Select(p => new
            {
                CharacterId = p.RevisionId,
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
    
    async Task<Dictionary<string, AccountName>> GetAccountNames()
    {
        var names = await _larpContext.Accounts.Find(_ => true)
            .Project(account => new AccountName()
            {
                AccountId = account.AccountId,
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
        
        foreach (var letters in info.Letters.GroupBy(x=>x.Value.TemplateId, x=>x.Value))
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

}