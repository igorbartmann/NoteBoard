using System;
using Moq;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Models.User;
using NoteBoard.Application.Services;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Tests.Services
{
    public class UserServiceTest
    {
        private IUserService _userService;
        private readonly Mock<ILoggedUserManager> _loggedUserManagerMock;
        private readonly Mock<IUserNormalizer> _userNormalizerMock;
        private readonly Mock<IUserValidator> _userValidatorMock;
        private readonly Mock<IUserMapper> _userMapperMock;
        private readonly Mock<IUserQuery> _userQueryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UserServiceTest()
        {
            _loggedUserManagerMock = new Mock<ILoggedUserManager>();
            _userNormalizerMock = new Mock<IUserNormalizer>();
            _userValidatorMock = new Mock<IUserValidator>();
            _userMapperMock = new Mock<IUserMapper>();
            _userQueryMock = new Mock<IUserQuery>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _userService = new UserService(
                _loggedUserManagerMock.Object, 
                _userNormalizerMock.Object, 
                _userValidatorMock.Object, 
                _userMapperMock.Object,
                _userQueryMock.Object, 
                _userRepositoryMock.Object, 
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnSuccess()
        {
            // Arrange
            var model = new UserCreateInputModel
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _userService.Create(model, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}