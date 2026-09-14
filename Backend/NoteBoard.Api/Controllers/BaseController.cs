using System;
using Microsoft.AspNetCore.Mvc;
using NoteBoard.Api.Common.Response;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.Common.Result;

namespace NoteBoard.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ActionResult NoteBoardCreatedAtAction<T>(string actionName, object routeValue, T data)
        {
            return CreatedAtAction(actionName, routeValue, new ApiResponse<T>(data, true, null));
        }

        protected ActionResult NoteBoardOk<T>(T data, string? message = null)
        {
            return Ok(new ApiResponse<T>(data, true, message));
        }

        protected ActionResult NoteBoardNotFound<T>(string? message)
        {
            return NotFound(new ApiResponse<T>(default, false, message));
        }

        protected ActionResult NoteBoardBadRequest<T>(string? message)
        {
            return BadRequest(new ApiResponse<T>(default, false, message));
        }

        protected ActionResult NoteBoardUnauthorized<T>(string? message = null)
        {
            return Unauthorized(new ApiResponse<T>(default, false, message));
        }

        protected ActionResult NoteBoardForbidden<T>(string? message = null)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden, 
                new ApiResponse<T>(default, false, message ?? ApplicationMessages.Forbidden));
        }

        protected ActionResult NoteBoardConcurrency<T>(string? message)
        {
            return Conflict(new ApiResponse<T>(default, false, message));
        }

        protected ActionResult NoteBoardInternalServerError<T>()
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<T>(default, false, null));
        }

        protected ActionResult ConvertToNoteBoardAction<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return NoteBoardOk(result.Data, result.Messages.FirstOrDefault()?.Message);
            }
            
            return result.Status switch
            {
                ResultStatus.NotFoundError => NoteBoardNotFound<T>(result.Messages.FirstOrDefault()?.Message),
                ResultStatus.ValidatorError => NoteBoardBadRequest<T>(string.Join(", ", result.Messages.Select(m => m.Message))),
                ResultStatus.AuthenticationError => NoteBoardUnauthorized<T>(result.Messages.FirstOrDefault()?.Message),
                ResultStatus.Forbidden => NoteBoardForbidden<T>(result.Messages.FirstOrDefault()?.Message),
                ResultStatus.ConcurrencyError => NoteBoardConcurrency<T>(result.Messages.FirstOrDefault()?.Message),
                _ => NoteBoardInternalServerError<T>()
            };
        }
    }
}