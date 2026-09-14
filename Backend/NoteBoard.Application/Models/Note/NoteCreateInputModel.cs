using System;
using NoteBoard.Domain.Enumerators;

namespace NoteBoard.Application.Models.Note
{
    public sealed record NoteCreateInputModel()
    {
        public string? Title {get;init;}
        public string? Content {get;init;}
        public NoteColor Color {get;init;}
    }
}