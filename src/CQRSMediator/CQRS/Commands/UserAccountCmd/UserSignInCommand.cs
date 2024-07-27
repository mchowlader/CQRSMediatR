using CQRSMediator.Controllers;
using CQRSMediator.Entities;
using CQRSMediator.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CQRSMediator.CQRS.Commands.UserAccountCmd;

public record UserSignInCommand : IRequest<SignResponse<Users>>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public bool IsRemember { get; set; }
}

public class UserSignInCommandHandler : IRequestHandler<UserSignInCommand, SignResponse<Users>>
{
    private readonly SignInManager<Users> _signInManager;
    private readonly UserManager<Users> _userManager;
    private readonly ILogger<UserSignInCommandHandler> _logger;

    public UserSignInCommandHandler(SignInManager<Users> signInManager, UserManager<Users> userManager, ILogger<UserSignInCommandHandler> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }
    public async Task<SignResponse<Users>> Handle(UserSignInCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user is not null)
            {
                var passwordValid = await _signInManager
                    .PasswordSignInAsync(user, command.Password, command.IsRemember, lockoutOnFailure: false);
                if (passwordValid is not null)
                {
                    _logger.LogInformation("User signed in.");

                    return new SignResponse<Users>
                    {
                        Success = true,
                        Data = user,
                        Message = "User signed in successfully."
                    };
                }
                else
                {
                    _logger.LogWarning("Invalid Login attempt.");

                    return new SignResponse<Users>
                    {
                        Success = false,
                        Data = null,
                        Message = "Invalid Login attempt."
                    };
                }
            }
            return new SignResponse<Users>
            {
                Success = false,
                Data = null,
                Message = "User not found"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while attempting to sign in user: {Email}", command.Email);

            return new SignResponse<Users>
            {
                Success = false,
                Data = null,
                Message = "Invalid Login attempt."
            };
        }
    }
}