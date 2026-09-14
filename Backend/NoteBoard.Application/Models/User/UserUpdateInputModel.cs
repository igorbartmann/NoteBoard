using System;
using NoteBoard.Application.Models.Note;

namespace NoteBoard.Application.Models.User
{
    public sealed record UserUpdateInputModel(int Id, string? Name, DateTimeOffset? UpdatedAt) : BaseUpdateInputModel(Id, UpdatedAt);
}