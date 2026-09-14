using System;
using NoteBoard.Application.Models.User;

namespace NoteBoard.Application.Interfaces.Services
{
    public interface IUserService : IBaseService<UserCreateInputModel, UserUpdateInputModel, UserViewModel>
    {
        
    }
}