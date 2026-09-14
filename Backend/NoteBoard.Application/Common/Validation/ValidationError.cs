using System;
using NoteBoard.Application.Common.Result;

namespace NoteBoard.Application.Common.Validation
{
    public sealed record ValidationError(string Field, string Message) : ResultMessage(Message);
}