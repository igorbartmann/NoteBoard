using System;

namespace NoteBoard.Application.Models.Auth
{
    public sealed record LoginInputModel(string Email, string Password);
}