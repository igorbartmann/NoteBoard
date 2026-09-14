using System;

namespace NoteBoard.Application.Common.Messages
{
    public static class ValidationMessages
    {
        public static string Required(string field) => $"{field} is required.";
        public static string MaxLength(string field, int maxLength) => $"{field} cannot exceed {maxLength} characters.";
        public const string InvalidId = "The id is invalid.";
        public const string EmailAlreadyInUse = "This email address is already in use by another account. Please use a different address or log in.";
        public const string InvalidEmail = "The email address format is invalid. Please enter a valid email address (e.g., name@example.com).";
        public const string InvalidPassword = "The password must include at least one uppercase letter, lowercase letter, number, and symbol.";
        public const string InvalidPasswordWhiteSpace = "The password cannot contain spaces at the beginning or the end.";
        public const string InvalidColor = "The color is invalid.";
    }   
}