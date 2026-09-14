using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteBoard.Data.Persistence.Constants;
using NoteBoard.Domain.Constants;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Data.Persistence.Configurations
{
    public sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable(TableNames.Note);
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName(ColumnNames.Id)
                .UseIdentityColumn(1,1)
                .IsRequired();

            builder.Property(u => u.Title)
                .HasColumnName(ColumnNames.Title)
                .HasMaxLength(NoteConstants.TitleMaxLength)
                .IsRequired();

            builder.Property(u => u.Content)
                .HasColumnName(ColumnNames.Content)
                .HasMaxLength(NoteConstants.ContentMaxLength)
                .IsRequired();

            builder.Property(u => u.IsCompleted)
                .HasColumnName(ColumnNames.IsCompleted)
                .IsRequired();

            builder.Property(u => u.Color)
                .HasColumnName(ColumnNames.Color)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasColumnName(ColumnNames.CreatedAt)
                .IsRequired();

            builder.Property(u => u.CreatedBy)
                .HasColumnName(ColumnNames.CreatedBy)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .HasColumnName(ColumnNames.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(n => n.CreatedByUser)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}