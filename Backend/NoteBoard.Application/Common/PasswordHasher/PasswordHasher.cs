using System;
using Microsoft.AspNetCore.Identity;

namespace NoteBoard.Application.Common.PasswordHasher
{
    public class PasswordHasher
    {
        private readonly PasswordHasher<string> _passwordHasher;

        public PasswordHasher()
        {
            _passwordHasher = new();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool CheckPassword(string hashedPassword, string rawPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, rawPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}