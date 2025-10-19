using Library.Application.CQRS;
using Library.Domain.Constants;
using Library.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Auth.Commands;

public class RegisterCommand : ICommand<RegisterResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public bool Succeeded { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

public class RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegisterResponse
            {
                Succeeded = false,
                Errors = ["User with this email already exists"]
            };
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true, // For simplicity, auto-confirm email
            MembershipDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // Add default Member role
            await userManager.AddToRoleAsync(user, UserRoles.Member);

            return new RegisterResponse
            {
                Succeeded = true,
                UserId = user.Id.ToString(),
                Email = user.Email
            };
        }

        return new RegisterResponse
        {
            Succeeded = false,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
}