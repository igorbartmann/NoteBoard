using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Models.User;
namespace NoteBoard.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly ILoggedUserManager _loggedUserManager;
        private readonly IUserQuery _query;
        private readonly IUserService _service;

        public UserController(ILoggedUserManager loggedUserManager, IUserQuery query, IUserService service)
        {
            _loggedUserManager = loggedUserManager;
            _query = query;
            _service = service;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            if (!_loggedUserManager.IsLoggedUserAuthenticated())
            {
                return NoteBoardUnauthorized<UserViewModel>();
            } 
            
            if (id != _loggedUserManager.GetLoggedUserId())
            {
                return NoteBoardForbidden<UserViewModel>();
            }

            var viewModel = await _query.GetByIdAsync(id, cancellationToken);
            if (viewModel is null)
            {
                return NoteBoardNotFound<UserViewModel>(ApplicationMessages.NotFound(nameof(User)));
            }

            return NoteBoardOk(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create(UserCreateInputModel model, CancellationToken cancellationToken)
        {
            var result = await _service.Create(model, cancellationToken);
            return ConvertToNoteBoardAction(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, UserUpdateInputModel model, CancellationToken cancellationToken)
        {
            if (id != model.Id)
            {
                return BadRequest(ValidationMessages.InvalidId);
            }

            var result = await _service.Edit(model, cancellationToken);
            return ConvertToNoteBoardAction(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _service.Delete(id, cancellationToken);
            return ConvertToNoteBoardAction(result);
        }
    }
}