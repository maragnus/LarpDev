using JetBrains.Annotations;
using Larp.Protos;
using Larp.WebService.ProtobufControllers;

namespace Larp.WebService.Controllers;

[PublicAPI]
public class AuthController : ProtobufController
{
    public Task<Account> GetAccount()
    {
        return Task.FromResult(new Account() { AccountId = "id", Created = "created", IsSuperAdmin = true });
    }

    public Account Boring()
    {
        return new Account() { AccountId = "id", Created = "created", IsSuperAdmin = true };
    }
}