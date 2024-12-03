using InternFselV2.Service.Command.UserCommands;
using InternFselV2.Service.Queries.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternFselV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result;
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            command.Id = id;
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result;
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetbyId([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetUserQuery {Id = id }).ConfigureAwait(false);
            return result;
        }
    }
}
