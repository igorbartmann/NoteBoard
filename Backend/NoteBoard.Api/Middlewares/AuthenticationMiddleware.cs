using System;
using System.Security.Claims;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Models.LoggedUser;

namespace NoteBoard.Api.Middlewares
{
    public class AuthenticationMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext httpContext, ILoggedUserManager loggedUserManager)
        {
            TrySetLoggedUser(httpContext, loggedUserManager);

            await next(httpContext);
        }

        private static void TrySetLoggedUser(HttpContext httpContext, ILoggedUserManager loggedUserManager)
        {
            var user = httpContext.User;
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return;
            }

            var idString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nameString = user.FindFirst(ClaimTypes.Name)?.Value;
            var emailString = user.FindFirst(ClaimTypes.Email)?.Value;

            if (Int32.TryParse(idString, out int idInteger)
                && !string.IsNullOrEmpty(nameString)
                && !string.IsNullOrEmpty(emailString))
            {
                int userId = idInteger;
                string userName = nameString;
                string userEmail = emailString;

                loggedUserManager.SetLoggedUser(new LoggedUser(userId, userName, userEmail));
            }
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}