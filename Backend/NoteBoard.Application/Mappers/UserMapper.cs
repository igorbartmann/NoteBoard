using System;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Models.User;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Mappers
{
    public sealed class UserMapper : IUserMapper
    {
        public User ToEntity(UserCreateInputModel model)
        {
            return new User(0, model.Name!, model.Email!, model.Password ?? string.Empty, DateTimeOffset.UtcNow, updatedAt: null, notes: []);
        }

        public User ToEntity(User entity, UserUpdateInputModel model)
        {
            entity.Name = model.Name!;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            return entity;
        }

        public UserViewModel ToViewModel(User entity)
        {
            return new UserViewModel(entity.Id, entity.Name, entity.Email, entity.CreatedAt, entity.UpdatedAt);
        }
    }
}