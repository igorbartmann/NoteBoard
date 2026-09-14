using System;

namespace NoteBoard.Application.Common.Result
{
    public sealed class Result<T>(ResultStatus status, T? data, IReadOnlyCollection<ResultMessage> messages)
    {
        public ResultStatus Status { get; } = status;
        public T? Data { get; } = data;
        public IReadOnlyCollection<ResultMessage> Messages {get;} = messages;
        public bool IsSuccess => Status == ResultStatus.Success;
        public bool IsFailure => !IsSuccess;        

        public static Result<T> Success(T? data = default, ResultMessage? message = null) => new (ResultStatus.Success, data, message is null ? [] : [message]);
        public static Result<T> NotFound(ResultMessage message) => new (ResultStatus.NotFoundError, default, [message]);
        public static Result<T> AuthenticationError(ResultMessage message) => new (ResultStatus.AuthenticationError, default, [message]);
        public static Result<T> Forbidden(ResultMessage message) => new (ResultStatus.Forbidden, default, [message]);
        public static Result<T> ConcurrencyError(ResultMessage message) => new (ResultStatus.ConcurrencyError, default, [message]);
        public static Result<T> ValidationError(ResultMessage message) => new (ResultStatus.ValidatorError, default, [message]);
        public static Result<T> ValidationError(IReadOnlyCollection<ResultMessage> messages) => new (ResultStatus.ValidatorError, default, messages);
    }
}