using CQRSMediator.CQRS.Commands.ProductCmd;
using CQRSMediator.CQRS.Queries;
using CQRSMediator.CQRS.Queries.ProductQue;
using CQRSMediator.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRSMediator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("All")]
        public async Task<IActionResult> Get([FromQuery]PaginationModel pagination)
        {
            var result = await _mediator.Send(new GetAllProductQuery(pagination));
            return Ok(result);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _mediator.Send(new GetProductByIdQuery { Id = id }));
        }

        [HttpPut("Id")]
        public async Task<IActionResult> Update(int id, UpdateProductCommand command)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("Id")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteProductByIdCommand { Id = id }));
        }
    }
}