using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicTacToe.Domain.Services.AuthService;

namespace TicTacToe.Web.Filter;

public class AuthFilter(IAuthService authService) : IAsyncAuthorizationFilter
{
    private readonly IAuthService _authService = authService;


    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            SetUnauthorizedResult(context, "Authorization header is missing");
            return;
        }

        var user = _authService.GetUserFromBase64(authHeader.ToString());

        if (user is null)
        {
            SetUnauthorizedResult(context, "Invalid authentication data");
            return;
        }

        var id = await _authService.AuthorizeUser(user);

        if (!id.HasValue)
        {
            SetUnauthorizedResult(context, "Invalid login or password");
            return;
        }

        context.HttpContext.Items["UserId"] = id;
    }


    private static void SetUnauthorizedResult(AuthorizationFilterContext context, string errorMessage)
    {
        context.Result = new ContentResult
        {
            Content = errorMessage,
            //Content = JsonSerializer.Serialize(new { error = errorMessage }),
            ContentType = "application/json",
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }
}
