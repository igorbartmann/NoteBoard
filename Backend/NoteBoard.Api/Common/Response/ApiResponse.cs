using System;

namespace NoteBoard.Api.Common.Response
{
    public sealed record ApiResponse<T>(T? Data, bool Success, string? Message);
}