using System.Security.Authentication;
using Grpc.Core;
using Larp.Data;
using Larp.Proto;
using Account = Larp.Data.Account;

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

public class UserGrpcService : LarpUserClient.LarpUserClientBase
{
    private readonly LarpContext _larpContext;

    public UserGrpcService(LarpContext larpContext)
    {
        _larpContext = larpContext;
    }
     
    public override Task<AccountResponse> GetAccount(Empty request, ServerCallContext context)
    {
        var account = context.GetAccount();
        return base.GetAccount(request, context);
    }
}

public class AdminGrpcService : Larp.Proto.LarpAdminClient.LarpAdminClientBase
{
    
}