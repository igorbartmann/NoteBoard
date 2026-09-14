using System;
using NoteBoard.Application.Models.User;

namespace NoteBoard.Application.Interfaces.Normalizers
{
    public interface IUserNormalizer : IBaseNormalizer<UserCreateInputModel, UserUpdateInputModel>
    {
        
    }
}