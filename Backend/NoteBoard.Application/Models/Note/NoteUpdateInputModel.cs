using System;
using NoteBoard.Domain.Enumerators;

namespace NoteBoard.Application.Models.Note
{
    public sealed record NoteUpdateInputModel(int Id, string? Title, string? Content, NoteColor Color, DateTimeOffset? UpdatedAt) : BaseUpdateInputModel(Id, UpdatedAt);
}