using System.Security.Authentication;
using Google.Protobuf.Collections;
using Grpc.Core;
using Larp.Data;
using Larp.Protos;
using Larp.Protos.Services;
using Account = Larp.Data.Account;
using AccountEmail = Larp.Protos.AccountEmail;

namespace Larp.WebService.GrpcServices;

public static class ServerCallContextExtensions
{
    public static Account GetAccount(this ServerCallContext context)
    {
        if (context.UserState["Account"] is not Account account)
            throw new AuthenticationException("Account is invalid");
        return account;
    }
}

public class UserGrpcService : LarpUser.LarpUserBase
{
    private readonly LarpContext _larpContext;

    public UserGrpcService(LarpContext larpContext)
    {
        _larpContext = larpContext;
    }
    
    public override Task<AccountResponse> GetAccount(Empty request, ServerCallContext context)
    {
        var account = context.GetAccount();

        var emails = account.Emails.Select(e => new Protos.AccountEmail()
        {
            Email = e.Email,
            IsPreferred = e.IsPreferred,
            IsVerified = e.IsVerified
        });

        var result = new Protos.Account()
        {
            AccountId = account.AccountId,
            Created = account.Created.ToString("O"),
            Location = account.Location ?? "",
            Name = account.Name ?? "",
            Phone = account.Phone ?? ""
        };
        result.Emails.AddRange(emails);
        
        return Task.FromResult(new AccountResponse() { Account = result });
    }
}

public class AdminGrpcService : Larp.Protos.Services.LarpAdmin.LarpAdminBase
{
    public override Task<AccountResponse> SetAccount(AccountRequest request, ServerCallContext context)
    {
        return base.SetAccount(request, context);
    }
}