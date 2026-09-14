using NoteBoard.Application.Mappers;
using NoteBoard.Application.Queries;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Enumerators;
using NoteBoard.Tests.Common;

namespace NoteBoard.Tests.Queries
{
    public class UserQueryTest
    {
        [Fact]
        public async Task ExistsByEmailAsyncToExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Users.Add(new User(1, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, null, notes: []));
            await context.SaveChangesAsync();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.ExistsByEmailAsync("alice@example.com", CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByEmailAsyncToNonExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.ExistsByEmailAsync("user@example.com", CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task GetByEmailAsyncToExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Users.Add(new User(7, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, null, notes: []));
            await context.SaveChangesAsync();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetByEmailAsync("alice@example.com", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(7, result.Id);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public async Task GetByEmailAsyncFailureToNonExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetByEmailAsync("user@example.com", CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserCredentialsByEmailAsyncToExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Users.Add(new User(3, "Alice", "alice@example.com", "hashed-password", DateTimeOffset.UtcNow, null, notes: []));
            await context.SaveChangesAsync();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetUserCredentialsByEmailAsync("alice@example.com", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("alice@example.com", result.Email);
            Assert.Equal("hashed-password", result.Password);
        }

        [Fact]
        public async Task GetByIdAsyncToExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Users.Add(new User(5, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, null, notes: []));
            await context.SaveChangesAsync();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetByIdAsync(5, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("Alice", result.Name);
            Assert.Equal("alice@example.com", result.Email);
        }

         [Fact]
        public async Task GetByIdAsyncToNonExistentUser()
        {
            using var context = SetupTest.CreateInMemoryContext();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetByIdAsync(2, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdWithNotesAsyncToUserWithNotes()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Users.Add(new User(10, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, null, notes: []));
            context.Notes.Add(new Note(1, "Ttitle 1", "Content 1", false, NoteColor.Yellow, DateTimeOffset.UtcNow, createdBy: 10, updatedAt: null));
            context.Notes.Add(new Note(2, "Title 2", "Content 2", false, NoteColor.Green, DateTimeOffset.UtcNow, createdBy: 10, updatedAt: null));
            await context.SaveChangesAsync();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetByIdWithNotesAsync(10, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal(2, result.Notes.Count());
        }

        [Fact]
        public async Task GetByIdWithNotesAsyncToUserWithoutNotes()
        {
            using var context = SetupTest.CreateInMemoryContext();

            context.Users.Add(new User(10, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, null, notes: []));
            await context.SaveChangesAsync();

            var userQuery = new UserQuery(context, new UserMapper());

            var result = await userQuery.GetByIdWithNotesAsync(10, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Empty(result.Notes);
        }
    }
}
