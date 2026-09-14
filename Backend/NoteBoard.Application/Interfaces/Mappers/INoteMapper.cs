using System;
using NoteBoard.Application.Models.Note;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Interfaces.Mappers
{
    public interface INoteMapper : IBaseMapper<Note, NoteCreateInputModel, NoteUpdateInputModel, NoteViewModel>
    {
        
    }
}