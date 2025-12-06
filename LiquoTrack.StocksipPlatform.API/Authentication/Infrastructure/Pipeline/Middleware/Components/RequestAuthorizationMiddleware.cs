using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Authorization;
using CustomAllowAnonymousAttribute = LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes.AllowAnonymousAttribute;
namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Components;

/**
 * RequestAuthorizationMiddleware is a custom middleware.
 * This middleware is used to authorize requests.
 * It validates a token is included in the request header and that the token is valid.
 * If the token is valid then it sets the user in HttpContext.Items["User"].
 */
public class RequestAuthorizationMiddleware(RequestDelegate next,
    ILogger<RequestAuthorizationMiddleware> logger)
{
    private readonly ILogger<RequestAuthorizationMiddleware> _logger = logger;

     /**
     * InvokeAsync is called by the ASP.NET Core runtime.
     * It is used to authorize requests.
     * It validates a token is included in the request header and that the token is valid.
     * If the token is valid then it sets the user in HttpContext.Items["User"].
     */
    public async Task InvokeAsync(
        HttpContext     context,
        IUserQueryService userQueryService,
        ITokenService     tokenService)
    {
        _logger.LogInformation("Entering InvokeAsync");
            
        if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        var requestPath = context.Request.Path.Value ?? string.Empty;
        if (requestPath.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
            requestPath.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
            requestPath.Equals("/", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        var endpoint  = context.GetEndpoint();
        var hasCustomAllowAnon = endpoint?.Metadata.Any(m => m is CustomAllowAnonymousAttribute) ?? false;
        var hasBuiltInAllowAnon = endpoint?.Metadata.Any(m => m is IAllowAnonymous || m is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute) ?? false;
        var allowAnon = hasCustomAllowAnon || hasBuiltInAllowAnon;

        _logger.LogInformation("Allow Anonymous = {AllowAnonymous}", allowAnon);
        if (allowAnon)
        {
            await next(context);
            return;
        }
        
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Split(' ') 
            .Last();

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Missing token");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or invalid token");
            return;
        }
        
        var userId = await tokenService.ValidateToken(token);
        if (userId is null)
        {
            _logger.LogWarning("Invalid token");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token");
            return;
        }
        
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId));
        _logger.LogInformation("Successful authorization. Setting HttpContext.Items[\"User\"]");
        context.Items["User"] = user;
        _logger.LogInformation("Continuing with Middleware Pipeline");
        // call next middleware
        await next(context);
    }
}