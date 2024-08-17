using CQRSMediator.Entities;
using CQRSMediator.Helper;
using CQRSMediator.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CQRSMediator.CQRS.Commands.UserAccountCmd;

public sealed record UserSignInCommand : IRequest<SignResponse<Users>>
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
    private readonly IPasswordHash _passwordHash;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserSignInCommandHandler(SignInManager<Users> signInManager
        , UserManager<Users> userManager
        , ILogger<UserSignInCommandHandler> logger
        , IPasswordHash passwordHash
        , IHttpContextAccessor contextAccessor)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _passwordHash = passwordHash;
        _contextAccessor = contextAccessor;
    }

    public async Task<SignResponse<Users>> Handle(UserSignInCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user is not null && command.Email != null && command.Password != null)
            {
                bool verified = _passwordHash.Verify(command.Password, user.PasswordHash!);
                if(verified)
                {
                    await _signInManager.SignInAsync(user, isPersistent: command.IsRemember);
                    var currentUser = _contextAccessor.HttpContext?.User;
                    if(_signInManager.IsSignedIn(currentUser))
                    {
                        _logger.LogInformation("User signed in Successfully.");

                        return new SignResponse<Users>
                        {
                            Success = true,
                            Data = new Users { UserName = user.UserName, Email = user.Email },
                            Message = "User signed in successfully."
                        };
                    }
                    else
                    {
                        _logger.LogWarning("Sign-in attempt failed unexpectedly.");
                        return new SignResponse<Users>
                        {
                            Success = false,
                            Data = null,
                            Message = "Sign-in attempt failed."
                        };
                    }
                }
                else
                {
                    _logger.LogInformation("Try to password match.");
                    return new SignResponse<Users>
                    {
                        Success = false,
                        Data = null,
                        Message = "Password incorrect!"
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