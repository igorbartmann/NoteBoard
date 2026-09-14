using System;
using System.Globalization;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Models.Note;

namespace NoteBoard.Application.Normalizers
{
    public sealed class NoteNormalizer : INoteNormalizer
    {
        public NoteCreateInputModel Normalize(NoteCreateInputModel model)
        {
            return model with
            {
                Title = NormalizeTitle(model.Title),
                Content = NormalizeContent(model.Content)
            };
        }

        public NoteUpdateInputModel Normalize(NoteUpdateInputModel model)
        {
            return model with
            {
                Title = NormalizeTitle(model.Title),
                Content = NormalizeContent(model.Content)
            };
        }

        public static string? NormalizeTitle(string? title)
        {        
            if (string.IsNullOrEmpty(title))
            {
                return title;
            }

            var textInfo = CultureInfo.InvariantCulture.TextInfo;
            return textInfo.ToTitleCase(title).Trim();
        }

        public static string? NormalizeContent(string? content)
        {
            return content?.Trim();
        }
    }
}