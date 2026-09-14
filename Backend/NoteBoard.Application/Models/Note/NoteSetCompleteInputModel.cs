using System;

namespace NoteBoard.Application.Models.Note
{
    public sealed record NoteSetCompleteInputModel(int Id, DateTimeOffset? UpdatedAt) : BaseUpdateInputModel(Id, UpdatedAt);
}