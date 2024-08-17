using CQRSMediator.CQRS.Commands.UserAccountCmd;
using CQRSMediator.Entities;
using CQRSMediator.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(CreateUserAccountCommand command)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for SignUp data.");
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            }
            else
            {
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
                    _logger.LogError($"Error {ex.Message}", ex);
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>(false, "An error occurred during SignUp.", null)
                    );
                }
            }
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserSignInCommand command)
        {
            if (!ModelState.IsValid)
            {
              
                _logger.LogWarning("Invalid model state for SignIn data.");
                return BadRequest(new ApiResponse<Users>(false, "An error occurred during SignIn.", null));
            }
            else
            {
                try
                {
                    var result = await _mediator.Send(command);
                    if (result.Success)
                    {
                        _logger.LogInformation(result.Message);
                        return Ok(
                            new ApiResponse<Users>
                            (
                                true,
                                result.Message is not null ? result.Message : "User signed in successfully.",
                                result.Data!
                            )
                        );

                    }
                    else
                    {
                        _logger.LogWarning("Invalid login attempt for user: {Email}", command.Email);
                        return BadRequest(
                            new ApiResponse<Users>
                            (
                                false,
                                result.Message is not null ? result.Message : "Invalid login attempt.",
                                null!
                            )
                        );
                    }
                }
                catch (Exception)
                {
                    _logger.LogWarning("Invalid login attempt for user: {Email}", command.Email);
                    return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    (
                        false, "An error occurred during Signip.", null!
                    ));
                }
            }

        }
    }
}