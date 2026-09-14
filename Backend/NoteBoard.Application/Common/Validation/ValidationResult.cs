using System;

namespace NoteBoard.Application.Common.Validation
{
    public sealed class ValidationResult
    {
        public ValidationResult(IReadOnlyCollection<ValidationError> errors)
        {
            Errors = errors;
        }

        public IReadOnlyCollection<ValidationError> Errors { get; }
        public bool HasError => Errors.Count > 0;
    }
}