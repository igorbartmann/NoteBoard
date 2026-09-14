using System;

namespace NoteBoard.Application.Common.Messages
{
    public static class ApplicationMessages
    {
        public static string NotFound(string parameter) => $"The {parameter} was not found!";
        public static string ConcurrencyError(string parameter) => $"This record was updated by another user. Please reload the {parameter} to get the latest version.";
        public const string Forbidden = "You do not have access to perform this action.";
        public const string NoteAlreadyCompleted = "The note is already marked as completed.";
        public const string AuthConfigurationError = "Authentication properties is not defined in the configuration file.";
        public const string InternalError = "An internal error has occurred. Please try again in a few minutes.";
        public const string UserNotAuthenticated = "You should be authenticated to perform this action.";
    }   
}