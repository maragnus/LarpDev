using Grpc.Core;
using Larp.Proto;
using Larp.Proto.Authorization;

namespace Larp.WebService.GrpcServices;

public class AuthenticationGrpcService : LarpAuthentication.LarpAuthenticationBase
{
    private readonly ILogger<AuthenticationGrpcService> _logger;

    public AuthenticationGrpcService(ILogger<AuthenticationGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<InitiateLoginResponse> InitiateLogin(InitiateLoginRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Login instantiated by {Email}", request.Email);
        return Task.FromResult(new InitiateLoginResponse()
        {
            Message = "Success",
            StatusCode = ValidationResponseCode.Success
        });
    }

    public override Task<ConfirmLoginResponse> ConfirmLogin(ConfirmLoginRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Login confirmed by {Email}", request.Email);
        return Task.FromResult(new ConfirmLoginResponse()
        {
            Message = "Success",
            Profile = new Account() { },
            SessionId = Guid.NewGuid().ToString(),
            StatusCode = ValidationResponseCode.Success
        });
    }

    public override Task<ValidateSessionResponse> ValidateSession(ValidateSessionRequest request, ServerCallContext context)
    {
        // TODO -- get email
        _logger.LogInformation("Session revalidated by {Email}", request.SessionId);
        return Task.FromResult(new ValidateSessionResponse()
        {
            Profile = new Account(),
            StatusCode = ValidationResponseCode.Success
        });
    }
}