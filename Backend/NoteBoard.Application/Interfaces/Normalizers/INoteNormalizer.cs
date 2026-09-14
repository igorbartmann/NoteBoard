using System;
using NoteBoard.Application.Models.Note;
namespace NoteBoard.Application.Interfaces.Normalizers
{
    public interface INoteNormalizer : IBaseNormalizer<NoteCreateInputModel, NoteUpdateInputModel>
    {
        
    }
}