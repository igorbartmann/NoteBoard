using System;
using NoteBoard.Domain.Enumerators;

namespace NoteBoard.Domain.Entities
{
    public class Note : BaseEntity
    {
        /// <summary>
        /// Empty constructor for Entity Framework Core.
        /// </summary>
        protected Note() : base() {}

        public Note(
            int id, 
            string title, 
            string content, 
            bool isCompleted,
            NoteColor color, 
            DateTimeOffset createdAt, 
            int createdBy, 
            DateTimeOffset? updatedAt) : base(id, createdAt, updatedAt)
        {
            Title = title;
            Content = content;
            IsCompleted = isCompleted;
            Color = color;
            CreatedBy = createdBy;
        }

        public string Title {get;set;} = null!;
        public string Content {get;set;} = null!;
        public bool IsCompleted {get;set;}
        public NoteColor Color {get;set;}
        public int CreatedBy {get;set;} 

        #region Navigation properties
        public User? CreatedByUser {get;set;}
        #endregion
    }
}