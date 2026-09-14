using Microsoft.EntityFrameworkCore;
using NoteBoard.Data.Persistence;

namespace NoteBoard.Tests.Common
{
    public static class SetupTest
    {
        public static NoteBoardDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<NoteBoardDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new NoteBoardDbContext(options);
        }
    }
}
