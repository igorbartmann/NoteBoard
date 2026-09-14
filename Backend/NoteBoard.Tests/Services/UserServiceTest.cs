using Moq;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Mappers;
using NoteBoard.Application.Models.User;
using NoteBoard.Application.Normalizers;
using NoteBoard.Application.Services;
using NoteBoard.Application.Validators;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<ILoggedUserManager> _loggedUserManagerMock;
        private readonly Mock<IUserQuery> _userQueryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _loggedUserManagerMock = new Mock<ILoggedUserManager>();
            _userQueryMock         = new Mock<IUserQuery>();
            _userRepositoryMock    = new Mock<IUserRepository>();
            _unitOfWorkMock        = new Mock<IUnitOfWork>();

            _userService = new UserService(
                loggedUserManager: _loggedUserManagerMock.Object,
                normalizer:        new UserNormalizer(),
                validator:         new UserValidator(),
                mapper:            new UserMapper(),
                query:             _userQueryMock.Object,
                repository:        _userRepositoryMock.Object,
                unitOfWork:        _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Create()
        {
            _userQueryMock
                .Setup(q => q.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var model = new UserCreateInputModel
            {
                Name = "alice",
                Email = "ALICE@EXAMPLE.COM",
                Password = "Password1!"
            };

            var result = await _userService.Create(model, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Alice", result.Data!.Name);
            Assert.Equal("alice@example.com", result.Data.Email);
            _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserWithInvalidEmail()
        {
            var model = new UserCreateInputModel
            {
                Name = "Alice",
                Email = "invalid",
                Password = "Password1!"
            };

            var result = await _userService.Create(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ValidatorError, result.Status);
        }

        [Fact]
        public async Task CreateUserWithAlreadyExistentEmail()
        {
            _userQueryMock
                .Setup(q => q.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var model = new UserCreateInputModel
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "Password1!"
            };

            var result = await _userService.Create(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ValidatorError, result.Status);
            _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateUserWithInvalidName()
        {
            var model = new UserCreateInputModel
            {
                Name = null,
                Email = "alice@example.com",
                Password = "Password1!"
            };

            var result = await _userService.Create(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Edit()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(true);
            _loggedUserManagerMock.Setup(l => l.GetLoggedUserId()).Returns(1);
            _userRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User(1, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), notes: []));

            var model = new UserUpdateInputModel(1, "Alice updated", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

            var result = await _userService.Edit(model, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Alice Updated", result.Data!.Name);
            _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditUserWithoutAuthentication()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(false);

            var model = new UserUpdateInputModel(1, "Alice", DateTimeOffset.UtcNow);

            var result = await _userService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.AuthenticationError, result.Status);
        }

        [Fact]
        public async Task EditUserDifferentFromUserLogged()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(true);
            _loggedUserManagerMock.Setup(l => l.GetLoggedUserId()).Returns(2);

            var model = new UserUpdateInputModel(1, "Alice", DateTimeOffset.UtcNow);

            var result = await _userService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.Forbidden, result.Status);
        }

        [Fact]
        public async Task EditUserToInexistentEntry()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(true);
            _loggedUserManagerMock.Setup(l => l.GetLoggedUserId()).Returns(1);
            _userRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var model = new UserUpdateInputModel(1, "Alice", DateTimeOffset.UtcNow);

            var result = await _userService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.NotFoundError, result.Status);
        }

        [Fact]
        public async Task EditUserToConcurrencyScenario()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(true);
            _loggedUserManagerMock.Setup(l => l.GetLoggedUserId()).Returns(1);
            _userRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User(1, "Alice", "alice@example.com", "hash", DateTimeOffset.UtcNow, new DateTimeOffset(2001, 1, 1, 12, 35, 0, TimeSpan.Zero), notes: []));

            var model = new UserUpdateInputModel(1, "Alice", new DateTimeOffset(2001, 1, 1, 12, 30, 0, TimeSpan.Zero));

            var result = await _userService.Edit(model, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.ConcurrencyError, result.Status);
        }

        [Fact]
        public async Task Delete()
        {
            var entity = new User(1, "Alice", "alice@example.com", "hash",
                DateTimeOffset.UtcNow, null, notes: []);

            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(true);
            _loggedUserManagerMock.Setup(l => l.GetLoggedUserId()).Returns(1);
            _userRepositoryMock
                .Setup(r => r.ReadByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _userService.Delete(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _userRepositoryMock.Verify(r => r.Remove(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteWithNotAuthenticatedUser()
        {
            _loggedUserManagerMock.Setup(l => l.IsLoggedUserAuthenticated()).Returns(false);

            var result = await _userService.Delete(1, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ResultStatus.AuthenticationError, result.Status);
        }
    }
}
