using Larp.Data;
using MongoDB.Bson;

namespace Larp.WebService.Tests;

public class TestDataHelper
{
    private readonly LarpContext _context;

    public TestDataHelper(LarpContext context)
    {
        _context = context;
    }

    public async Task AddAccount(string name, string email, DateTimeOffset created)
    {
        var account = new Data.Account()
        {
            AccountId = ObjectId.GenerateNewId().ToString(),
            Name = name,
            Emails = { new Data.AccountEmail() { Email = email, IsVerified = true } },
            Created = created
        };

        await _context.Accounts.InsertOneAsync(account);
    }
}