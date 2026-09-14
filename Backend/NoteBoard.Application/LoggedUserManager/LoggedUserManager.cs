using System;
using NoteBoard.Application.Models.LoggedUser;

namespace NoteBoard.Application.LoggedUserManager
{
    public class LoggedUserManager : ILoggedUserManager
    {
        private LoggedUser? _loggedUser;

        public void SetLoggedUser(LoggedUser loggedUser)
        {
            _loggedUser = loggedUser;
        }

        public LoggedUser? GetLoggedUser()
        {
            return _loggedUser;
        }

        public bool IsLoggedUserAuthenticated()
        {
            return _loggedUser != null;
        }

        public int? GetLoggedUserId()   
        {
            return _loggedUser?.Id;
        }

        public string? GetLoggedUserName()
        {
            return _loggedUser?.Name;
        }

        public string? GetLoggedUserEmail()
        {
            return _loggedUser?.Email;
        }
    }
}