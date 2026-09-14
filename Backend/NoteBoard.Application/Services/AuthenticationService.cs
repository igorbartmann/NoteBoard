using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.PasswordHasher;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Models.Auth;
using NoteBoard.Application.Models.User;

namespace NoteBoard.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserQuery _userQuery;
        private readonly PasswordHasher _passwordHasher;

        public AuthenticationService(IConfiguration configuration, IUserQuery userQuery)
        {
            _configuration = configuration;
            _userQuery = userQuery;
            _passwordHasher = new();
        }

        public async Task<Result<LoginViewModel>> LoginAsync(LoginInputModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return Result<LoginViewModel>.AuthenticationError(new ResultMessage("Invalid credentials"));
            }

            var userCredentials = await _userQuery.GetUserCredentialsByEmailAsync(model.Email, cancellationToken);
            if (userCredentials is null)
            {
                return Result<LoginViewModel>.AuthenticationError(new ResultMessage("Invalid credentials"));
            }

            if (!_passwordHasher.CheckPassword(userCredentials.PasswordHash, model.Password))
            {
                return Result<LoginViewModel>.AuthenticationError(new ResultMessage("Invalid credentials"));
            }

            var viewModel = GenerateTokens(userCredentials);

            return Result<LoginViewModel>.Success(viewModel);
        }

        public async Task<Result<LoginViewModel>> RefreshTokenAsync(int userId, string email, CancellationToken cancellationToken)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(email))
            {
                return Result<LoginViewModel>.AuthenticationError(new ResultMessage("Invalid credentials"));
            }

            var userCredentials = await _userQuery.GetUserCredentialsByEmailAsync(email, cancellationToken);
            if (userCredentials is null || userCredentials.Id != userId)
            {
                return Result<LoginViewModel>.AuthenticationError(new ResultMessage("Invalid credentials"));
            }

            var viewModel = GenerateTokens(userCredentials);

            return Result<LoginViewModel>.Success(viewModel);
        }

        private LoginViewModel GenerateTokens(UserCredentialsValueObject userCredentials)
        {
            const int RefreshTokenExpirationExtensionMinutes = 30;

            var issuer = _configuration["Authentication:Issuer"];
            var audience = _configuration["Authentication:Audience"];
            var signingKey = _configuration["Authentication:SigningKey"];

            if (string.IsNullOrWhiteSpace(issuer) 
                || string.IsNullOrWhiteSpace(audience)
                || string.IsNullOrWhiteSpace(signingKey)
                || !int.TryParse(_configuration["Authentication:ExpirationMinutes"], out int tokenExpirationMinutes)
                || tokenExpirationMinutes == 0)
            {
                throw new InvalidOperationException(ApplicationMessages.AuthConfigurationError);
            }

            var claims = new Dictionary<string, object>
            {
                [ClaimTypes.NameIdentifier] = userCredentials.Id.ToString(),
                [ClaimTypes.Name] = userCredentials.Name,
                [ClaimTypes.Email] = userCredentials.Email
            };

            var jsonWebTokenHandler = new JsonWebTokenHandler();

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256);           

            var token = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
                SigningCredentials = credentials  
            };
            
            var refreshToken = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes + RefreshTokenExpirationExtensionMinutes),
                SigningCredentials = credentials  
            };

            return new LoginViewModel(
                AccessToken: jsonWebTokenHandler.CreateToken(token),
                RefreshToken: jsonWebTokenHandler.CreateToken(refreshToken)
            );
        }
    }
}