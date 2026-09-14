using System;
using NoteBoard.Application.Common.Validation;

namespace NoteBoard.Application.Interfaces.Validators
{
    public interface IBaseValidator<TCreateInputModel, TUpdateInputModel>
    {
        ValidationResult Validate(TCreateInputModel model);
        ValidationResult Validate(TUpdateInputModel model);
    }
}