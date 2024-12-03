using InternFselV2.Service.Command.ProductCommands;
using InternFselV2.Service.Queries.ProductCommands;
using InternFselV2.Service.Queries.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternFselV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result;
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            command.Id = id;
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result;
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetbyId([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetProductQuery { Id = id }).ConfigureAwait(false);
            return result;
        }
    }
}
