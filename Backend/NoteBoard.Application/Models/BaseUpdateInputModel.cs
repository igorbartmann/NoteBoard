using System;

namespace NoteBoard.Application.Models.Note
{
    public abstract record BaseUpdateInputModel(int Id, DateTimeOffset? UpdatedAt);
}