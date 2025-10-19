using Library.Application.Auth.Commands;
using Library.Application.Auth.Queries;
using Library.Domain.Constants;
using MediatR;

namespace Library.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/api/auth")
            .WithTags("Authentication & User Management")
            .WithOpenApi();

        auth.MapPost("/register", Register)
            .WithName("Register")
            .Accepts<RegisterCommand>("application/json")
            .Produces<RegisterResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .AllowAnonymous();

        auth.MapPost("/login", Login)
            .WithName("Login")
            .Accepts<LoginCommand>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .AllowAnonymous();

        auth.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .Produces<object>(StatusCodes.Status200OK)
            .RequireAuthorization();

        // User management endpoints
        auth.MapGet("/users", GetAllUsers)
            .WithName("GetAllUsers")
            .Produces<List<UserDto>>(StatusCodes.Status200OK)
            .RequireAuthorization(policy => policy.RequireRole(UserRoles.Admin));

        auth.MapPut("/users/{userId}/roles", UpdateUserRoles)
            .WithName("UpdateUserRoles")
            .Accepts<ChangeUserRolesRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(UserRoles.Admin));
    }

    private static async Task<IResult> Register(RegisterCommand command, IMediator mediator)
    {
        var response = await mediator.Send(command);

        if (!response.Succeeded)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Errors", response.Errors.ToArray() }
            });
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> Login(LoginCommand command, IMediator mediator)
    {
        var response = await mediator.Send(command);
        return Results.Ok(response);
    }

    private static IResult GetCurrentUser(HttpContext httpContext)
    {
        var user = httpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            return Results.Unauthorized();
        }

        var roles = user.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return Results.Ok(new
        {
            Email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            UserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Roles = roles,
        });
    }

    private static async Task<IResult> GetAllUsers(IMediator mediator)
    {
        var query = new GetAllUsersQuery();
        var users = await mediator.Send(query);
        return Results.Ok(users);
    }

    private static async Task<IResult> UpdateUserRoles(
        Guid userId,
        ChangeUserRolesRequest request,
        IMediator mediator)
    {
        var command = new ChangeUserRoleCommand
        {
            UserId = userId,
            RolesToAdd = request.RolesToAdd ?? new List<string>(),
            RolesToRemove = request.RolesToRemove ?? new List<string>()
        };

        var result = await mediator.Send(command);

        if (result)
        {
            return Results.Ok(new { message = "User roles updated successfully" });
        }

        return Results.Problem("Failed to update user roles");
    }
}

public record ChangeUserRolesRequest(List<string>? RolesToAdd, List<string>? RolesToRemove);