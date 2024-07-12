using CQRSMediator.CQRS.Commands.UserAccountCmd;
using CQRSMediator.Entities;
using CQRSMediator.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CQRSMediator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private IMediator _mediator;
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<UserAccountController> _logger;

        public UserAccountController(
            IMediator mediator,
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            ILogger<UserAccountController> logger)
        {
            _mediator = mediator;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(CreateUserAccountCommand command)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for SignUp data.");
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            }
            try
            {
                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    _logger.LogInformation(result.Message);
                    return Ok(new ApiResponse<Users>(true, result.Message, result.Data));
                }

                return BadRequest(new ApiResponse<object>(false, result.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error {ex.Message}", ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                new ApiResponse<object>(false, "An error occurred during SignUp.", null)
                );
            }
        }
    }
}