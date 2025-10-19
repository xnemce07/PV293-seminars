using Library.Application.CQRS;
using Library.Application.Repositories;
using Library.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Auth.Queries;

public class GetAllUsersQuery : IQuery<List<UserDto>>
{
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class GetAllUsersQueryHandler(IUserRepository repository)
    : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetAllWithRolesAsync(cancellationToken);
    }
}
