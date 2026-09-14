using System;
using NoteBoard.Application.Models.Note;

namespace NoteBoard.Application.Interfaces.Validators
{
    public interface INoteValidator : IBaseValidator<NoteCreateInputModel, NoteUpdateInputModel>
    {
        
    }
}