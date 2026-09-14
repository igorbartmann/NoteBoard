using System;
using NoteBoard.Application.Common.Result;

namespace NoteBoard.Application.Interfaces.Services
{
    public interface IBaseService<TCreateInputModel, TUpdateInputModel, TViewModel>
    {
        Task<Result<TViewModel>> Create(TCreateInputModel model, CancellationToken cancellationToken);

        Task<Result<TViewModel>> Edit(TUpdateInputModel model, CancellationToken cancellationToken);

        Task<Result<TViewModel>> Delete(int id, CancellationToken cancellationToken);
    }
}