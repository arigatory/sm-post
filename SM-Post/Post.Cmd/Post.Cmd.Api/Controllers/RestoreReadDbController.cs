using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RestoreReadDbController : ControllerBase
{

    private readonly ILogger<RestoreReadDbController> _logger;
    private readonly IMediator _mediator;

    public RestoreReadDbController(ILogger<RestoreReadDbController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RestoreReadDbAsync()
    {
        try
        {
            await _mediator.Send(new RestoreReadDbCommand());

            return StatusCode(StatusCodes.Status201Created, new BaseResponse
            {
                Message = "Read database restore request completed successfully!",
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to restore read database!";
            _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}
