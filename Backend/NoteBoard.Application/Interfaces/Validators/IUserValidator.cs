using System;
using NoteBoard.Application.Models.User;

namespace NoteBoard.Application.Interfaces.Validators
{
    public interface IUserValidator : IBaseValidator<UserCreateInputModel, UserUpdateInputModel>
    {
        
    }
}