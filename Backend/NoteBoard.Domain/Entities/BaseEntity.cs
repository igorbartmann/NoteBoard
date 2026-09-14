using System;

namespace NoteBoard.Domain.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Empty constructor for Entity Framework Core.
        /// </summary>
        protected BaseEntity() { }
        
        public BaseEntity(int id, DateTimeOffset createdAt, DateTimeOffset? updatedAt)
        {
            Id = id;    
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public int Id {get;set;}
        public DateTimeOffset CreatedAt {get;set;}
        public DateTimeOffset? UpdatedAt {get;set;}
    }
}