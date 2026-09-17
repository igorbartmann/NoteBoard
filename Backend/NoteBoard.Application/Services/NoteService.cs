using System;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Models.Note;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Application.Services
{
    public sealed class NoteService : BaseService<Note, NoteCreateInputModel, NoteUpdateInputModel, NoteViewModel>, INoteService
    {
        private readonly ILoggedUserManager _loggedUserManager;

        public NoteService(ILoggedUserManager loggedUserManager, INoteNormalizer normalizer, INoteValidator validator, INoteMapper mapper, INoteRepository repository, IUnitOfWork unitOfWork) : base(normalizer, validator, mapper, repository, unitOfWork)
        {
            _loggedUserManager = loggedUserManager;
        }

        public override async Task<Result<NoteViewModel>> Create(NoteCreateInputModel model, CancellationToken cancellationToken)
        {
            if (!_loggedUserManager.IsLoggedUserAuthenticated() || _loggedUserManager.GetLoggedUserId() == 0)
            {
                return Result<NoteViewModel>.AuthenticationError(new ResultMessage(ApplicationMessages.UserNotAuthenticated));
            }

            model = _normalizer.Normalize(model);

            var validationResult = _validator.Validate(model);
            if (validationResult.HasError)
            {
                return Result<NoteViewModel>.ValidationError(validationResult.Errors);
            }

            var entity = _mapper.ToEntity(model);
            entity.CreatedBy = _loggedUserManager.GetLoggedUserId()!.Value;

            _repository .Add(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = _mapper.ToViewModel(entity);

            return Result<NoteViewModel>.Success(viewModel);
        }

        public override async Task<Result<NoteViewModel>> Edit(NoteUpdateInputModel model, CancellationToken cancellationToken)
        {
            model = _normalizer.Normalize(model);

            var validationResult = _validator.Validate(model);
            if (validationResult.HasError)
            {
                return Result<NoteViewModel>.ValidationError(validationResult.Errors);
            }

            var entity = await _repository.ReadByIdAsync(model.Id, cancellationToken);
            if (entity is null)
            {
                return Result<NoteViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(Note))));
            }

            if (entity.CreatedBy != _loggedUserManager.GetLoggedUserId())
            {
                return Result<NoteViewModel>.Forbidden(new ResultMessage(ApplicationMessages.Forbidden));
            }

            if (entity.UpdatedAt.HasValue && entity.UpdatedAt != model.UpdatedAt)
            {
                return Result<NoteViewModel>.ConcurrencyError(new ResultMessage(ApplicationMessages.ConcurrencyError(nameof(Note))));
            }

            entity = _mapper.ToEntity(entity, model);          

            _repository.Update(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = _mapper.ToViewModel(entity);
            
            return Result<NoteViewModel>.Success(viewModel);
        }

        public async Task<Result<NoteViewModel>> SetCompleted(NoteSetCompleteInputModel model, CancellationToken cancellationToken)
        {
            var entity = await _repository.ReadByIdAsync(model.Id, cancellationToken);
            if (entity is null)
            {
                return Result<NoteViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(Note))));
            }

            if (entity.CreatedBy != _loggedUserManager.GetLoggedUserId())
            {
                return Result<NoteViewModel>.Forbidden(new ResultMessage(ApplicationMessages.Forbidden));
            }

            if (entity.UpdatedAt != model.UpdatedAt)
            {
                return Result<NoteViewModel>.ConcurrencyError(new ResultMessage(ApplicationMessages.ConcurrencyError(nameof(Note))));
            }

            if (entity.IsCompleted)
            {
                return Result<NoteViewModel>.ValidationError([new ResultMessage(ApplicationMessages.NoteAlreadyCompleted)]);
            }

            entity.IsCompleted = true;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _repository.Update(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = _mapper.ToViewModel(entity);
            
            return Result<NoteViewModel>.Success(viewModel);
        }

        public override async Task<Result<NoteViewModel>> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _repository.ReadByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                return Result<NoteViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(Note))));
            }

            if (entity.CreatedBy != _loggedUserManager.GetLoggedUserId())
            {
                return Result<NoteViewModel>.Forbidden(new ResultMessage(ApplicationMessages.Forbidden));
            }

            _repository.Remove(entity);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            return Result<NoteViewModel>.Success(message: new ResultMessage(ApplicationMessages.SuccessfullyDeleted));
        }
    }
}