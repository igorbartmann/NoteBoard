using System;

namespace NoteBoard.Application.Interfaces.Normalizers
{
    public interface IBaseNormalizer<TCreateInputModel, TUpdateInputModel>
    {
        TCreateInputModel Normalize(TCreateInputModel model);

        TUpdateInputModel Normalize(TUpdateInputModel model);
    }
}