using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteBoard.Data.Persistence.Constants;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Constants;

namespace NoteBoard.Data.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableNames.User);
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Id)
                .HasColumnName(ColumnNames.Id)
                .UseIdentityColumn(1,1)
                .IsRequired();

            builder.Property(u => u.Name)
                .HasColumnName(ColumnNames.Name)
                .HasMaxLength(UserConstants.NameMaxLength)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName(ColumnNames.Email)
                .HasMaxLength(UserConstants.EmailMaxLength)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName(ColumnNames.PasswordHash)
                .HasMaxLength(UserConstants.PasswordHashMaxLength)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasColumnName(ColumnNames.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .HasColumnName(ColumnNames.UpdatedAt)
                .IsRequired(false);
        }
    }
}