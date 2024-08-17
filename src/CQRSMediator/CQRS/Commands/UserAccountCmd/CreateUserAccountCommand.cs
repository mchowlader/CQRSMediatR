using CQRSMediator.Entities;
using CQRSMediator.Helper;
using CQRSMediator.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CQRSMediator.CQRS.Commands.UserAccountCmd;

public sealed record CreateUserAccountCommand : IRequest<SignResponse<Users>>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserAccountCommandHandler : IRequestHandler<CreateUserAccountCommand, SignResponse<Users>>
{
    private readonly UserManager<Users> _userManager;
    private readonly ILogger<UserAccountCommandHandler> _logger;
    private readonly IPasswordHash _passwordHash;

    public UserAccountCommandHandler(UserManager<Users> userManager, ILogger<UserAccountCommandHandler> logger, IPasswordHash passwordHash)
    {
        _userManager = userManager;
        _logger = logger;
        _passwordHash = passwordHash;
    }

    public async Task<SignResponse<Users>> Handle(CreateUserAccountCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
        {
            _logger.LogWarning("Received null CreateUserAccountCommand.");
            return new SignResponse<Users>
            {
                Data = null,
                Success = false,
                Message = "Invalid data provider."
            };
        }
        try
        {
            var userExiting = await _userManager.FindByEmailAsync(command.Email);
            if (userExiting is not null)
            {
                var exitingResponse = new Users()
                {
                    Email = command.Email,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    ConcurrencyStamp = string.Empty
                };
                return new SignResponse<Users>
                {
                    Data = exitingResponse,
                    Success = true,
                    Message = "User or Email already exits"
                };
            }
            else
            {
                return await UserSignUpAsync(command);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An error occures while createing the user.");
            return new SignResponse<Users>
            {
                Data = null,
                Success = false,
                Message = "An error occurred while creating the user. Please try again later."
            };
        }
    }
    private async Task<SignResponse<Users>> UserSignUpAsync(CreateUserAccountCommand command)
    {
        Users user = new();
        user.Email = command.Email;
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.UserName = command.Email;
        user.PasswordHash = _passwordHash.Hash(command.Password);


        try
        {
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                var userData = await _userManager.FindByIdAsync(user.Id.ToString());

                //add Claim
                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, command.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var claimResult = await _userManager.AddClaimsAsync(user, claim);

                if (!claimResult.Succeeded)
                {
                    _logger
                        .LogWarning("Failed to add claims to user.{0}",
                        string.Join("; ", claimResult.Errors.Select(e => e.Description)));
                }

                if (userData is null)
                {
                    _logger.LogError($"User created but could not be found by ID: {user.Id}");
                    return new SignResponse<Users>
                    {
                        Data = null,
                        Success = false,
                        Message = "User creation succeeded but retrieval failed."
                    };
                }

                var response = new Users()
                {
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    UserName = userData.Email,
                    PhoneNumber = userData.PhoneNumber,
                    Email = userData.Email,
                    EmailConfirmed = userData.EmailConfirmed,
                    ConcurrencyStamp = string.Empty,
                };

                return new SignResponse<Users>
                {
                    Data = response,
                    Success = true,
                    Message = "User sign up successfully."
                };
            }
            else
            {
                _logger.LogWarning("User creation failed. Error: {0}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));

                return new SignResponse<Users>
                {
                    Data = null,
                    Success = false,
                    Message = "User creation failed. Error: {0}" +
                    string.Join("; ", result.Errors.Select(e => e.Description))
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("User creation failed. Error: {0}", ex);
            return new SignResponse<Users>
            {
                Success = false,
                Data = null,
                Message = "Internal Server error."
            };
        }
    }
}