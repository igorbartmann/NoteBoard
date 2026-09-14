using System;

namespace NoteBoard.Application.Models.User
{
    public sealed record UserViewModel(int Id, string Name, string Email, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);
}