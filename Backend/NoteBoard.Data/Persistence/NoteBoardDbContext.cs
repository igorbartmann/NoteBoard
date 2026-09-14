using System;
using Microsoft.EntityFrameworkCore;
using NoteBoard.Data.Persistence.Configurations;
using NoteBoard.Data.Persistence.Constants;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Data.Persistence
{
    public class NoteBoardDbContext(DbContextOptions<NoteBoardDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users {get;set;}
        public DbSet<Note> Notes {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DatabaseSchema.NoteBoard);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new NoteConfiguration());
        }
    }
}