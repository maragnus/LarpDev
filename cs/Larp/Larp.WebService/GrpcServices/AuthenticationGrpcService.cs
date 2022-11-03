using Grpc.Core;
using Larp.Proto.Authorization;
using Larp.WebService.LarpServices;
using Account = Larp.Proto.Account;

namespace Larp.WebService.GrpcServices;

public class AuthenticationGrpcService : LarpAuthentication.LarpAuthenticationBase
{
    private readonly ILogger<AuthenticationGrpcService> _logger;
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationGrpcService(ILogger<AuthenticationGrpcService> logger,
        IAuthenticationService authenticationService)
    {
        _logger = logger;
        _authenticationService = authenticationService;
       
    }

    public override async Task<InitiateLoginResponse> InitiateLogin(InitiateLoginRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Login instantiated by {Email}", request.Email);
        try
        {
            var result = await _authenticationService.InitiateLogin(request.Email, context.Peer);

            return new InitiateLoginResponse()
            {
                Message = result.IsSuccess ? "Success" : result.Message,
                StatusCode = result.IsSuccess ? ValidationResponseCode.Success : ValidationResponseCode.Invalid
            };
        }
        catch (Exception ex)
        {
            return new InitiateLoginResponse()
            {
                Message = ex.Message,
                StatusCode = ValidationResponseCode.Invalid
            };
        }
    }

    public override async Task<ConfirmLoginResponse> ConfirmLogin(ConfirmLoginRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Login confirmed by {Email}", request.Email);
        try
        {
            var result = await _authenticationService.ConfirmLogin(request.Email, request.Code);

            return new ConfirmLoginResponse()
            {
                Message = result.IsSuccess ? "Success" : result.Message,
                StatusCode = result.IsSuccess ? ValidationResponseCode.Success : ValidationResponseCode.Invalid,
                SessionId = result.SessionId,
                Profile = new Account() // TODO -- get account
            };
        }
        catch (Exception ex)
        {
            return new ConfirmLoginResponse()
            {
                Message = ex.Message,
                StatusCode = ValidationResponseCode.Invalid,
                SessionId = null
            };
        }
    }

    public override async Task<ValidateSessionResponse> ValidateSession(ValidateSessionRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Session revalidated by {SessionId}", request.SessionId);
        try
        {
            var result = await _authenticationService.ValidateSession(request.SessionId);

            var status = ValidationResponseCode.Success;
            if (result.IsExpired)
                status = ValidationResponseCode.Expired;
            else if (!result.IsSuccess)
                status = ValidationResponseCode.Invalid;

            return new ValidateSessionResponse()
            {
                StatusCode = status,
                Profile = new Account() // TODO -- get account
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception when validating session");
            return new ValidateSessionResponse()
            {
                StatusCode = ValidationResponseCode.Invalid
            };
        }
    }
}