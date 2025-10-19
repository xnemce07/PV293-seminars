using Library.Application.CQRS;
using Library.Domain.Constants;
using Library.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Auth.Commands;

public class ChangeUserRoleCommand : ICommand<bool>
{
    public Guid UserId { get; set; }
    public List<string> RolesToAdd { get; set; } = new();
    public List<string> RolesToRemove { get; set; } = new();
}

public class ChangeUserRoleCommandHandler(
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<ChangeUserRoleCommand, bool>
{
    public async Task<bool> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new ArgumentException($"User with ID {request.UserId} not found");
        }

        // Validate all roles exist
        foreach (var role in request.RolesToAdd.Concat(request.RolesToRemove))
        {
            if (!UserRoles.AllRoles.Contains(role))
            {
                throw new ArgumentException($"Invalid role: {role}");
            }
        }

        // Remove specified roles
        if (request.RolesToRemove.Any())
        {
            var result = await userManager.RemoveFromRolesAsync(user, request.RolesToRemove);
            if (!result.Succeeded)
            {
                return false;
            }
        }

        // Add specified roles
        if (request.RolesToAdd.Any())
        {
            var result = await userManager.AddToRolesAsync(user, request.RolesToAdd);
            if (!result.Succeeded)
            {
                return false;
            }
        }

        return true;
    }
}