using Grpc.Core;
using Grpc.Core.Interceptors;
using Larp.Data.Services;

namespace Larp.WebService;

class AuthInterceptor : Interceptor
{
    private readonly UserSessionManager _userSessionManager;
    private readonly ILogger<AuthInterceptor> _logger;
    
    public AuthInterceptor(UserSessionManager userSessionManager, ILogger<AuthInterceptor> logger)
    {
        _userSessionManager = userSessionManager;
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var authenticationHeader = context.RequestHeaders.Get("Authentication").Value;
        var result = await _userSessionManager.ValidateUserSession(authenticationHeader);
        if (result != UserSessionValidationResult.Authenticated)
            throw new Exception("Not authenticated");

        context.UserState["Account"] = _userSessionManager.GetUserAccount();
        
        _logger.LogInformation("GRPC: {MethodName}", context.Method ?? "Unknown");
        return await continuation(request, context);
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
       
        return continuation(request, context);
    }
}