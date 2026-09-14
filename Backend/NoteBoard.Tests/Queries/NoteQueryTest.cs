using NoteBoard.Application.Mappers;
using NoteBoard.Application.Queries;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Enumerators;
using NoteBoard.Tests.Common;

namespace NoteBoard.Tests.Queries
{
    public class NoteQueryTest
    {
        [Fact]
        public async Task GetByIdAsyncToExistentNote()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Notes.Add(new Note(1, "Buy milk", "Market", false, NoteColor.Yellow, DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            await context.SaveChangesAsync();

            var noteQuery = new NoteQuery(context, new NoteMapper());

            var result = await noteQuery.GetByIdAsync(1, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Buy milk", result.Title);
            Assert.Equal("Market", result.Content);
        }

        [Fact]
        public async Task GetByIdAsyncToNonExistentNote()
        {
            using var context = SetupTest.CreateInMemoryContext();

            var noteQuery = new NoteQuery(context, new NoteMapper());

            var result = await noteQuery.GetByIdAsync(999, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByOwnerIdAsyncToExistentNotes()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Notes.Add(new Note(1, "T1", "C1", false, NoteColor.Yellow, DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(2, "T2", "C2", false, NoteColor.Green,  DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(3, "T3", "C3", true, NoteColor.Red,    DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(4, "T4", "C4", false, NoteColor.White,  DateTimeOffset.UtcNow, createdBy: 2, updatedAt: null));
            context.Notes.Add(new Note(5, "T5", "C5", false, NoteColor.White,  DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            await context.SaveChangesAsync();

            var noteQuery = new NoteQuery(context, new NoteMapper());

            var result = await noteQuery.GetByOwnerIdAsync(1, CancellationToken.None);

            var list = result.ToList();
            Assert.Equal(4, list.Count);
            Assert.All(list, n => Assert.Equal(1, n.CreatedBy));
        }

        [Fact]
        public async Task GetByOwnerIdAsyncToNonExistentNotes()
        {
            using var context = SetupTest.CreateInMemoryContext();

            var noteQuery = new NoteQuery(context, new NoteMapper());

            var result = await noteQuery.GetByOwnerIdAsync(99, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIncompleteByOwnerIdAsync()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Notes.Add(new Note(1, "T1", "C1", isCompleted: true,  NoteColor.Yellow, DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(2, "T2", "C2", isCompleted: false,  NoteColor.Green,  DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(3, "T3", "C3", isCompleted: true, NoteColor.Red, DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(4, "T4", "C4", isCompleted: false, NoteColor.White, DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            context.Notes.Add(new Note(5, "T5", "C5", isCompleted: true, NoteColor.Red, DateTimeOffset.UtcNow, createdBy: 1, updatedAt: null));
            await context.SaveChangesAsync();

            var noteQuery = new NoteQuery(context, new NoteMapper());

            var result = await noteQuery.GetIncompleteByOwnerIdAsync(1, CancellationToken.None);

            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, n => Assert.False(n.IsCompleted));
        }
    }
}
