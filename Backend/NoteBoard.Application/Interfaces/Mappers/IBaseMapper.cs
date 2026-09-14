using System;

namespace NoteBoard.Application.Interfaces.Mappers
{
    public interface IBaseMapper<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel>
    {
        TEntity ToEntity(TCreateInputModel model);

        TEntity ToEntity(TEntity entity, TUpdateInputModel model);

        TViewModel ToViewModel(TEntity entity);
    }
}