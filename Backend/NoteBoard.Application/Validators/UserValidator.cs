using System;
using System.Net.Mail;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.Validation;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.Models.User;
using NoteBoard.Domain.Constants;

namespace NoteBoard.Application.Validators
{
    public sealed class UserValidator : IUserValidator
    {
        private static readonly char[] ValidPasswordSymbols = ['.', '!', '@', '#', '$', '&', '*', '-'];

        public ValidationResult Validate(UserCreateInputModel model)
        {
            var errors = new List<ValidationError>();

            ValidateName(nameof(model.Name), model.Name, errors);

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                errors.Add(new ValidationError(nameof(model.Email), ValidationMessages.Required(nameof(model.Email))));
            }
            else if (model.Email.Length > UserConstants.EmailMaxLength)
            {
                errors.Add(new ValidationError(nameof(model.Email), ValidationMessages.MaxLength(nameof(model.Email), UserConstants.EmailMaxLength)));
            }
            else if (!MailAddress.TryCreate(model.Email, out _))
            {
                errors.Add(new ValidationError(nameof(model.Email), ValidationMessages.InvalidEmail));
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                errors.Add(new ValidationError(nameof(model.Password), ValidationMessages.Required(nameof(model.Password))));
            }
            else if (model.Password.Length != model.Password.Trim().Length)
            {
                errors.Add(new ValidationError(nameof(model.Password), ValidationMessages.InvalidPasswordWhiteSpace));
            }
            else if (model.Password.Length > UserConstants.PasswordMaxLength)
            {
                errors.Add(new ValidationError(nameof(model.Password), ValidationMessages.MaxLength(nameof(model.Password), UserConstants.PasswordMaxLength)));
            }
            else if (!model.Password.Any(char.IsDigit) || !model.Password.Any(char.IsUpper) || !model.Password.Any(char.IsLower) || !model.Password.Any(ValidPasswordSymbols.Contains))
            {
                errors.Add(new ValidationError(nameof(model.Password), ValidationMessages.InvalidPassword));
            }

            return new ValidationResult(errors);
        }

        public ValidationResult Validate(UserUpdateInputModel model)
        {
            var errors = new List<ValidationError>();

            if (model.Id <= 0)
            {
                errors.Add(new ValidationError(nameof(model.Id), ValidationMessages.InvalidId));
            }

            ValidateName(nameof(model.Name), model.Name, errors);

            if (model.UpdatedAt is null)
            {
                errors.Add(new ValidationError(nameof(model.UpdatedAt), ValidationMessages.Required(nameof(model.UpdatedAt))));
            }

            return new ValidationResult(errors);
        }

        private static void ValidateName(string fieldName, string? value, List<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.Required(fieldName)));
            }
            else if (value.Length > UserConstants.NameMaxLength)
            {
                errors.Add(new ValidationError(fieldName, ValidationMessages.MaxLength(fieldName, UserConstants.NameMaxLength)));
            }
        }
    }
}