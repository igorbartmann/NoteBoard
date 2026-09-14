using System;

namespace NoteBoard.Application.Models.Auth
{
    public sealed record LoginViewModel(string AccessToken, string RefreshToken);
}