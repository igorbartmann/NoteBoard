using System;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Models.Note;

namespace NoteBoard.Application.Interfaces.Services
{
    public interface INoteService : IBaseService<NoteCreateInputModel, NoteUpdateInputModel, NoteViewModel>
    {
        Task<Result<NoteViewModel>> SetCompleted(NoteSetCompleteInputModel model, CancellationToken cancellationToken);
    }
}