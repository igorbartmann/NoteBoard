using System;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Models.Auth;

namespace NoteBoard.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<Result<LoginViewModel>> LoginAsync(LoginInputModel model, CancellationToken cancellationToken);

        Task<Result<LoginViewModel>> RefreshTokenAsync(int userId, string email, CancellationToken cancellationToken);
    }
}