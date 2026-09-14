using System;

namespace NoteBoard.Application.Common.Result
{
    public enum ResultStatus
    {
        Success = 1,
        ValidatorError = 2,
        NotFoundError = 3,
        AuthenticationError = 4,
        Forbidden = 5,
        ConcurrencyError = 6
    }
}