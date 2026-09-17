using Moq;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Mappers;
using NoteBoard.Application.Models.Note;
using NoteBoard.Application.Normalizers;
using NoteBoard.Application.Services;
using NoteBoard.Application.Validators;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Enumerators;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Tests.Services
{
    public class NoteServiceTest
    {
        private readonly Mock<ILoggedUserManager> _loggedUserManagerMock;
        private readonly Mock<INoteRepository> _noteRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly NoteService _noteService;

        public NoteServiceTest()
        {
            _loggedUserManagerMock = new Mock<ILoggedUserManager>();
            _noteRepositoryMock    = new Mock<INoteRepository>();
            _unitOfWorkMock        = new Mock<IUnitOfWork>();
            
            _noteService = new NoteService(
                loggedUserManager: _loggedUserManagerMock.Object,
                normalizer:        new NoteNormalizer(),
                validator:         new NoteValidator(),
                mapper:            new NoteMapper(),
                repository:        _noteRepositoryMock.Object,
                unitOfWork:        _unitOfWorkMock.Object);
        }

        private void DefineUserAsAuthenticated(int userId = 1)
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(true);
            _loggedUserManagerMock.Setup(l => l.GetLoggedUserId()).Returns(userId);
        }

        [Fact]
        public async Task Create()
        {
            DefineUserAsAuthenticated(userId: 10);

            var model = new NoteCreateInputModel
            {
                Title = "bUy milk",
                Content = "Market",
                Color = NoteColor.Yellow
            };

            var result = await _noteService.Create(model, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Buy Milk", result.Data!.Title);
            Assert.Equal(10, result.Data.CreatedBy);
            _noteRepositoryMock.Verify(r => r.Add(It.IsAny<Note>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateWithtouAuthentication()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(false);

            var model = new NoteCreateInputModel
            {
                Title = "Buy milk",
                Content = "Market",
                Color = NoteColor.Yellow
            };

            var result = await _noteService.Create(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.AuthenticationError, result.Status);
        }

        [Fact]
        public async Task CreateWithInvalidTitle()
        {
            DefineUserAsAuthenticated();

            var model = new NoteCreateInputModel
            {
                Title = "",
                Content = "market",
                Color = NoteColor.Yellow
            };

            var result = await _noteService.Create(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ValidatorError, result.Status);
            _noteRepositoryMock.Verify(r => r.Add(It.IsAny<Note>()), Times.Never);
        }

        [Fact]
        public async Task CreateWithInvalidColor()
        {
            DefineUserAsAuthenticated();

            var model = new NoteCreateInputModel
            {
                Title = "Buy milk",
                Content = "Market",
                Color = 0
            };

            var result = await _noteService.Create(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ValidatorError, result.Status);
        }

        [Fact]
        public async Task Edit()
        {
            DefineUserAsAuthenticated(userId: 1);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Note(
                    1,
                    "Title", 
                    "Content", 
                    false, NoteColor.Yellow,
                    DateTimeOffset.UtcNow, 
                    createdBy: 1,
                    new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero)
                ));

            var model = new NoteUpdateInputModel(1, "new title", "new content", NoteColor.Green, new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero));

            var result = await _noteService.Edit(model, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("New Title", result.Data!.Title);
            Assert.Equal(NoteColor.Green, result.Data.Color);
            _noteRepositoryMock.Verify(r => r.Update(It.IsAny<Note>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditWithInvalidTitle()
        {
            DefineUserAsAuthenticated();

            var model = new NoteUpdateInputModel(1, string.Empty, "market", NoteColor.Yellow, DateTimeOffset.UtcNow);

            var result = await _noteService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ValidatorError, result.Status);
        }

        [Fact]
        public async Task EditNonExistentEntry()
        {
            DefineUserAsAuthenticated();

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Note?)null);

            var model = new NoteUpdateInputModel(1, "Buy Milk", "market", NoteColor.Yellow, DateTimeOffset.UtcNow);

            var result = await _noteService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.NotFoundError, result.Status);
        }

        [Fact]
        public async Task EditNoteFromOtherUser()
        {
            DefineUserAsAuthenticated(userId: 1);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Note(
                    1, 
                    "Buy Milk", 
                    "Market", 
                    false, 
                    NoteColor.Yellow,
                    DateTimeOffset.UtcNow, 
                    createdBy: 99, 
                    new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero)
                ));

            var model = new NoteUpdateInputModel(1, "Buy Milk", "market", NoteColor.Yellow, new DateTimeOffset(2025, 1, 1, 12, 30, 0, TimeSpan.Zero));

            var result = await _noteService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.Forbidden, result.Status);
        }

        [Fact]
        public async Task EditWithConcurrencyConflict()
        {
            DefineUserAsAuthenticated(userId: 1);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Note(
                    1, 
                    "Buy Milk", 
                    "Market", 
                    false, 
                    NoteColor.Yellow,
                    DateTimeOffset.UtcNow, 
                    createdBy: 1, 
                    updatedAt: new DateTimeOffset(2001, 1, 1, 12, 35, 0, TimeSpan.Zero)
                ));

            var model = new NoteUpdateInputModel(1, "Buy Milk", "Market", NoteColor.Yellow, new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero));

            var result = await _noteService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ConcurrencyError, result.Status);
        }

        [Fact]
        public async Task SetCompleted()
        {
            DefineUserAsAuthenticated(userId: 1);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Note(
                    1, 
                    "Buy Milk", 
                    "Market", 
                    isCompleted: false, 
                    NoteColor.Yellow,
                    DateTimeOffset.UtcNow, 
                    createdBy: 1, 
                    new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
                ));

            var model = new NoteSetCompleteInputModel(1, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

            var result = await _noteService.SetCompleted(model, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.True(result.Data!.IsCompleted);
            _noteRepositoryMock.Verify(r => r.Update(It.IsAny<Note>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetCompletedWhenAlreadyCompleted()
        {
            DefineUserAsAuthenticated(userId: 1);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Note(
                    5, 
                    "Buy Milk", 
                    "Market", 
                    isCompleted: true, 
                    NoteColor.Yellow,
                    DateTimeOffset.UtcNow, 
                    createdBy: 1, 
                    new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero)
                ));

            var model = new NoteSetCompleteInputModel(5, new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero));

            var result = await _noteService.SetCompleted(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ValidatorError, result.Status);
        }

        [Fact]
        public async Task Delete()
        {
            DefineUserAsAuthenticated(userId: 1);

            var note = new Note(
                5, 
                "Buy Milk", 
                "Market", 
                false,
                NoteColor.Yellow,
                DateTimeOffset.UtcNow, 
                createdBy: 1, 
                updatedAt: DateTimeOffset.UtcNow);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(note);

            var result = await _noteService.Delete(5, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _noteRepositoryMock.Verify(r => r.Remove(note), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteNonExistentNote()
        {
            DefineUserAsAuthenticated(userId: 1);

            _noteRepositoryMock
                .Setup(r => r.ReadByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Note?)null);

            var result = await _noteService.Delete(99, CancellationToken.None);

            Assert.True(result.IsFailure);
            _noteRepositoryMock.Verify(r => r.Remove(It.IsAny<Note>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
