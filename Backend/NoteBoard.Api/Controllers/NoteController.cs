using System;
using Microsoft.AspNetCore.Mvc;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Models.Note;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Api.Controllers
{
    [ApiController]
    [Route("api/notes")]
    public class NoteController : BaseController
    {
        private readonly ILoggedUserManager _loggedUserManager;
        private readonly INoteQuery _query;
        private readonly INoteService _service;

        public NoteController(ILoggedUserManager loggedUserManager, INoteQuery query, INoteService service)
        {
            _loggedUserManager = loggedUserManager;
            _query = query;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
        {
            int loggedUserId = _loggedUserManager.GetLoggedUserId() ?? 0;
            if (!_loggedUserManager.IsLoggedUserAuthenticated() || loggedUserId == 0)
            {
                return NoteBoardUnauthorized<NoteViewModel>();
            } 

            var notes = await _query.GetByOwnerIdAsync(loggedUserId, cancellationToken);
            return NoteBoardOk(notes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var note = await _query.GetByIdAsync(id, cancellationToken);
            if (note is null)
            {
                return NoteBoardNotFound<NoteViewModel>(ApplicationMessages.NotFound(nameof(Note)));
            }

            if (note.CreatedBy != _loggedUserManager.GetLoggedUserId())
            {
                return NoteBoardForbidden<NoteViewModel>();
            }

            return NoteBoardOk(note);
        }

        [HttpPost]
        public async Task<ActionResult> Create(NoteCreateInputModel model, CancellationToken cancellationToken)
        {
            var result = await _service.Create(model, cancellationToken);
            if (result.IsFailure)
            {
                return ConvertToNoteBoardAction(result);
            }

            return NoteBoardCreatedAtAction(nameof(GetById), new { Id = result.Data!.Id }, result.Data);
        }
        
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Edit(int id, NoteUpdateInputModel model, CancellationToken cancellationToken)
        {
            if (id != model.Id)
            {
                return BadRequest(ValidationMessages.InvalidId);
            }

            var result = await _service.Edit(model, cancellationToken);
            return ConvertToNoteBoardAction(result);
        }

        [HttpPut("{id:int}/complete")]
        public async Task<ActionResult> Complete(int id, NoteSetCompleteInputModel model, CancellationToken cancellationToken)
        {
            if (id != model.Id)
            {
                return BadRequest(ValidationMessages.InvalidId);
            }

            var result = await _service.SetCompleted(model, cancellationToken);
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