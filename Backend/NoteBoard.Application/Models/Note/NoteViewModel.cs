using System;
using NoteBoard.Domain.Enumerators;

namespace NoteBoard.Application.Models.Note
{
    public sealed record NoteViewModel(int Id, string Title, string Content, bool IsCompleted, NoteColor Color, DateTimeOffset CreatedAt, int CreatedBy, DateTimeOffset? UpdatedAt);
}
