using System;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.Validation;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.Models.Note;
using NoteBoard.Domain.Constants;
using NoteBoard.Domain.Enumerators;

namespace NoteBoard.Application.Validators
{
    public sealed class NoteValidator : INoteValidator
    {
        public ValidationResult Validate(NoteCreateInputModel model)
        {
            var errors = new List<ValidationError>();
            
            ValidateTitle(nameof(model.Title), model.Title, errors);

            ValidateContent(nameof(model.Content), model.Content, errors);

            ValidateColor(nameof(model.Color), model.Color, errors);

            return new ValidationResult(errors);
        }

        public ValidationResult Validate(NoteUpdateInputModel model)
        {
            var errors = new List<ValidationError>();

            if (model.Id <= 0)
            {
                errors.Add(new ValidationError(nameof(model.Id), ValidationMessages.InvalidId));
            }

            ValidateTitle(nameof(model.Title), model.Title, errors);

            ValidateContent(nameof(model.Content), model.Content, errors);

            ValidateColor(nameof(model.Color), model.Color, errors);

            if (model.UpdatedAt is null)
            {
                errors.Add(new ValidationError(nameof(model.UpdatedAt), ValidationMessages.Required(nameof(model.UpdatedAt))));
            }

            return new ValidationResult(errors);
        }

        private static void ValidateTitle(string fieldName, string? fieldValue, List<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.Required(fieldName)));
            }
            else if (fieldValue.Length > NoteConstants.TitleMaxLength)
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.MaxLength(fieldName, NoteConstants.TitleMaxLength)));
            }
        }

        private static void ValidateContent(string fieldName, string? fieldValue, List<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.Required(fieldName)));
            }
            else if (fieldValue.Length > NoteConstants.ContentMaxLength)
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.MaxLength(fieldName, NoteConstants.ContentMaxLength)));
            }
        }

        private static void ValidateColor(string fieldName, NoteColor fieldValue, List<ValidationError> errors)
        {
            if (!Enum.IsDefined(fieldValue))
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.InvalidColor));
            }
        }
    }
}