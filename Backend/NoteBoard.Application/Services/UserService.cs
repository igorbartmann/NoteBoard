using System;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.PasswordHasher;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Models.User;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Application.Services
{
    public sealed class UserService : BaseService<User, UserCreateInputModel, UserUpdateInputModel, UserViewModel>, IUserService
    {
        private readonly ILoggedUserManager _loggedUserManager;
        private readonly IUserQuery _query;
        private readonly PasswordHasher _passwordHasher;

        public UserService(ILoggedUserManager loggedUserManager, IUserQuery query, IUserNormalizer normalizer, IUserValidator validator, IUserMapper mapper, IUserRepository repository, IUnitOfWork unitOfWork) : base(normalizer, validator, mapper, repository, unitOfWork)
        {
            _loggedUserManager = loggedUserManager;
            _query = query;
            _passwordHasher = new();
        }

        public override async Task<Result<UserViewModel>> Create(UserCreateInputModel model, CancellationToken cancellationToken)
        {
            model = _normalizer.Normalize(model);

            var validationResult = _validator.Validate(model);
            if (validationResult.HasError)
            {
                return Result<UserViewModel>.ValidationError(validationResult.Errors);
            }

            var userAlreadyExists =  await _query.ExistsByEmailAsync(model.Email!, cancellationToken);
            if (userAlreadyExists)
            {
                return Result<UserViewModel>.ValidationError(new ResultMessage(ValidationMessages.EmailAlreadyInUse));
            }

            var entity = _mapper.ToEntity(model);
            entity.PasswordHash = _passwordHasher.HashPassword(model.Password!);

            _repository.Add(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = await _query.GetByIdAsync(entity.Id, cancellationToken);
            
            return Result<UserViewModel>.Success(data: viewModel);
        }

        public override async Task<Result<UserViewModel>> Edit(UserUpdateInputModel model, CancellationToken cancellationToken)
        {
            if (!_loggedUserManager.IsLoggedUserAuthenticated())
            {
                return Result<UserViewModel>.AuthenticationError(new ResultMessage(ApplicationMessages.UserNotAuthenticated));
            }
            
            if (model.Id != _loggedUserManager.GetLoggedUserId())
            {
                return Result<UserViewModel>.Forbidden(new ResultMessage(ApplicationMessages.Forbidden));
            }

            model = _normalizer.Normalize(model);

            var validationResult = _validator.Validate(model);
            if (validationResult.HasError)
            {
                return Result<UserViewModel>.ValidationError(validationResult.Errors);
            }

            var entity = await _repository.ReadByIdAsync(model.Id, cancellationToken);
            if (entity is null)
            {
                return Result<UserViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(User))));
            }

            if (entity.UpdatedAt.HasValue && entity.UpdatedAt != model.UpdatedAt)
            {
                return Result<UserViewModel>.ConcurrencyError(new ResultMessage(ApplicationMessages.ConcurrencyError(nameof(User))));
            }

            entity = _mapper.ToEntity(entity, model);

            _repository.Update(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = await _query.GetByIdAsync(entity.Id, cancellationToken);
            
            return Result<UserViewModel>.Success(data: viewModel);
        }

        public override async Task<Result<UserViewModel>> Delete(int id, CancellationToken cancellationToken)
        {
            if (!_loggedUserManager.IsLoggedUserAuthenticated())
            {
                return Result<UserViewModel>.AuthenticationError(new ResultMessage(ApplicationMessages.UserNotAuthenticated));
            }

            if (id != _loggedUserManager.GetLoggedUserId())
            {
                return Result<UserViewModel>.Forbidden(new ResultMessage(ApplicationMessages.Forbidden));
            }

            var entity = await _repository.ReadByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                return Result<UserViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(User))));
            }

            _repository.Remove(entity);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            return Result<UserViewModel>.Success(message: new ResultMessage(ApplicationMessages.SuccessfullyDeleted));
        }
    }
}