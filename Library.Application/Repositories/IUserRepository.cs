using Library.Application.Auth.Queries;
using Library.Domain.Entities;

namespace Library.Application.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllWithRolesAsync(CancellationToken cancellationToken);
    }
}
