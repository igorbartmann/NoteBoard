using System;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Models.Note;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Mappers
{
    public sealed class NoteMapper : INoteMapper
    {
        public Note ToEntity(NoteCreateInputModel model)
        {
            return new Note(0, model.Title!, model.Content!, false, model.Color, DateTimeOffset.UtcNow, createdBy: 0, updatedAt: null);
        }

        public Note ToEntity(Note entity, NoteUpdateInputModel model)
        {
            entity.Title = model.Title!;
            entity.Content = model.Content!;
            entity.Color = model.Color;
            entity.UpdatedAt = DateTimeOffset.UtcNow;       

            return entity;
        }

        public NoteViewModel ToViewModel(Note entity)
        {
            return new NoteViewModel(entity.Id, entity.Title, entity.Content, entity.IsCompleted, entity.Color, entity.CreatedAt, entity.CreatedBy, entity.UpdatedAt);
        }
    }
}