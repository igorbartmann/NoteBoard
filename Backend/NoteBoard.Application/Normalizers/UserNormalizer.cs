using System;
using System.Globalization;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Models.User;

namespace NoteBoard.Application.Normalizers
{
    public sealed class UserNormalizer : IUserNormalizer
    {
        public UserCreateInputModel Normalize(UserCreateInputModel model)
        {
            return model with
            {
                Name = NormalizeName(model.Name),
                Email = NormalizeEmail(model.Email)
            };
        }

        public UserUpdateInputModel Normalize(UserUpdateInputModel model)
        {
            return model with
            {
                Name = NormalizeName(model.Name)
            };
        }

        public static string? NormalizeName(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var textInfo = CultureInfo.InvariantCulture.TextInfo;
            return textInfo.ToTitleCase(name).Trim();
        }

        public static string? NormalizeEmail(string? email)
        {
            return email?.Trim().ToLower();
        }
    }
}