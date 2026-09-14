using System;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.Result;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.Models.Note;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Application.Services
{
    public abstract class BaseService<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel> : IBaseService<TCreateInputModel, TUpdateInputModel, TViewModel> 
        where TEntity : BaseEntity
        where TUpdateInputModel : BaseUpdateInputModel
    {
        protected readonly IBaseNormalizer<TCreateInputModel, TUpdateInputModel> _normalizer;
        protected readonly IBaseValidator<TCreateInputModel, TUpdateInputModel> _validator;
        protected readonly IBaseMapper<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel> _mapper;
        protected readonly IBaseRepository<TEntity> _repository;
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(
            IBaseNormalizer<TCreateInputModel, TUpdateInputModel> normalizer, 
            IBaseValidator<TCreateInputModel, TUpdateInputModel> validator, 
            IBaseMapper<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel> mapper, 
            IBaseRepository<TEntity> repository, 
            IUnitOfWork unitOfWork)
        {
            _normalizer = normalizer;
            _validator = validator;
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task<Result<TViewModel>> Create(TCreateInputModel model, CancellationToken cancellationToken)
        {
            model = _normalizer.Normalize(model);

            var validationResult = _validator.Validate(model);
            if (validationResult.HasError)
            {
                return Result<TViewModel>.ValidationError(validationResult.Errors);
            }

            var entity = _mapper.ToEntity(model);

            _repository.Add(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = _mapper.ToViewModel(entity);

            return Result<TViewModel>.Success(viewModel);
        }

        public virtual async Task<Result<TViewModel>> Edit(TUpdateInputModel model, CancellationToken cancellationToken)
        {
            model = _normalizer.Normalize(model);

            var validationResult = _validator.Validate(model);
            if (validationResult.HasError)
            {
                return Result<TViewModel>.ValidationError(validationResult.Errors);
            }

            var entity = await _repository.ReadByIdAsync(model.Id, cancellationToken);
            if (entity is null)
            {
                return Result<TViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(Note))));
            }

            if (entity.UpdatedAt != model.UpdatedAt)
            {
                return Result<TViewModel>.ConcurrencyError(new ResultMessage(ApplicationMessages.ConcurrencyError(nameof(Note))));
            }

            entity = _mapper.ToEntity(entity, model);          

            _repository.Update(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            var viewModel = _mapper.ToViewModel(entity);
            
            return Result<TViewModel>.Success(viewModel);
        }

        public virtual async Task<Result<TViewModel>> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _repository.ReadByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                return Result<TViewModel>.NotFound(new ResultMessage(ApplicationMessages.NotFound(nameof(Note))));
            }

            _repository.Remove(entity);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            return Result<TViewModel>.Success();
        }
    }
}