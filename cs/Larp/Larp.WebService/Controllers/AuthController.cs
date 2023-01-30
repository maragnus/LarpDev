using JetBrains.Annotations;
using Larp.Notify;
using Larp.Protos;
using Larp.Protos.Authorization;
using Larp.WebService.LarpServices;
using Larp.WebService.ProtobufControllers;

namespace Larp.WebService.Controllers;

[PublicAPI]
public class AuthController : ProtobufController
{
    private readonly IAuthenticationService _authenticationService;
    private readonly INotifyService _notifyService;

    public AuthController(IAuthenticationService authenticationService, INotifyService notifyService)
    {
        _authenticationService = authenticationService;
        _notifyService = notifyService;
    }

    public async Task<InitiateLoginResponse> InitiateLogin(InitiateLoginRequest request)
    {
        Logger.LogInformation("Login instantiated by {Email}", request.Email);
        try
        {
            var result = await _authenticationService.InitiateLogin(request.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

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

    public async Task<ConfirmLoginResponse> ConfirmLogin(ConfirmLoginRequest request)
    {
        Logger.LogInformation("Login confirmed by {Email}", request.Email);
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

    public async Task<ValidateSessionResponse> ValidateSession(ValidateSessionRequest request)
    {
        Logger.LogInformation("Session revalidated by {SessionId}", request.SessionId);
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
            Logger.LogError(ex, "Exception when validating session");
            return new ValidateSessionResponse()
            {
                StatusCode = ValidationResponseCode.Invalid
            };
        }
    }

    public async Task<LogoutResponse> Logout(LogoutRequest request)
    {
        await _authenticationService.DestroySession(request.SessionId);
        return new LogoutResponse();
    }
}