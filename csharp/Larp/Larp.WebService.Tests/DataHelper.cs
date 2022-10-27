using Larp.Data;
using Larp.Proto;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Larp.WebService.Tests;

public class DataHelper
{
    private readonly LarpContext _context;

    public DataHelper(LarpContext context)
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