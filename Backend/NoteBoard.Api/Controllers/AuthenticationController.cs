using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Models.Auth;

namespace NoteBoard.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginInputModel model, CancellationToken cancellationToken)
        {
            var result = await _service.LoginAsync(model, cancellationToken);
            if (result.IsFailure)
            {
                return NoteBoardUnauthorized<LoginViewModel?>(result.Messages.FirstOrDefault()?.Message);
            }

            return NoteBoardOk(result.Data);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return NoteBoardUnauthorized<LoginViewModel>();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (!int.TryParse(userIdString, out var userId) || string.IsNullOrEmpty(email))
            {
                return NoteBoardUnauthorized<LoginViewModel>();
            }

            var result = await _service.RefreshTokenAsync(userId, email, cancellationToken);
            if (result.IsFailure)
            {
                return NoteBoardUnauthorized<LoginViewModel>(result.Messages.FirstOrDefault()?.Message);
            }

            return NoteBoardOk(result.Data);
        }
    }
}