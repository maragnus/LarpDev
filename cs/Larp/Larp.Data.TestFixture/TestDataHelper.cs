using MongoDB.Bson;

namespace Larp.Data.TestFixture;

public class TestDataHelper
{
    private readonly LarpContext _context;

    public TestDataHelper(LarpContext context)
    {
        _context = context;
    }

    public async Task<string> AddAccount(string name, string email, DateTimeOffset created)
    {
        var account = new Data.Account()
        {
            AccountId = ObjectId.GenerateNewId().ToString(),
            Name = name,
            Emails = { new Data.AccountEmail()
            {
                Email = email,
                NormalizedEmail = email.ToLowerInvariant(),
                IsVerified = true
            } },
            Created = created
        };

        await _context.Accounts.InsertOneAsync(account);

        return account.AccountId;
    }
}