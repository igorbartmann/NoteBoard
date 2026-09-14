using System;
using NoteBoard.Application.Models.Note;
namespace NoteBoard.Application.Models.User
{
    public sealed record UserWithNotesViewModel(int Id, string Name, string Email, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt, IEnumerable<NoteViewModel> Notes);
}