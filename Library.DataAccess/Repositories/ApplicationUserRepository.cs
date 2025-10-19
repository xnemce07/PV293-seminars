using Library.Application.Auth.Queries;
using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repositories
{
    public class ApplicationUserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task<List<UserDto>> GetAllWithRolesAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MembershipDate = user.MembershipDate,
                    Roles = dbContext.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(dbContext.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name!)
                        .ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
