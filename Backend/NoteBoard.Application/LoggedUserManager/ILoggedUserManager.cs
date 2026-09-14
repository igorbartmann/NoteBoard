using System;
using NoteBoard.Application.Models.LoggedUser;

namespace NoteBoard.Application.LoggedUserManager
{
    public interface ILoggedUserManager
    {
        LoggedUser? GetLoggedUser();

        void SetLoggedUser(LoggedUser loggedUser);

        bool IsLoggedUserAuthenticated();

        int? GetLoggedUserId();

        string? GetLoggedUserName();
        
        string? GetLoggedUserEmail();
    }
}