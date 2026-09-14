using System;
using NoteBoard.Application.Models.User;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Interfaces.Mappers
{
    public interface IUserMapper : IBaseMapper<User, UserCreateInputModel, UserUpdateInputModel, UserViewModel>
    {
        
    }
}