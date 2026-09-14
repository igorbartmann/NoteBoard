using System;

namespace NoteBoard.Application.Models.User
{
    public sealed record UserCredentialsValueObject(int Id, string Name, string Email, string Password);
}