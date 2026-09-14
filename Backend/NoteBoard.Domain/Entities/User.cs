using System;

namespace NoteBoard.Domain.Entities
{
    public class User: BaseEntity
    {
        /// <summary>
        /// Empty constructor for Entity Framework Core.
        /// </summary>
        protected User() : base() {}

        public User(int id, string name, string email, string passwordHash, DateTimeOffset createdAt, DateTimeOffset? updatedAt, ICollection<Note> notes) : base(id, createdAt, updatedAt)
        {
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Notes = notes;
        }

        public string Name {get;set;} = null!;
        public string Email {get;set;} = null!;
        public string PasswordHash {get;set;} = null!;

        #region Navigation properties
        public ICollection<Note> Notes {get;set;} = [];
        #endregion
    }
}