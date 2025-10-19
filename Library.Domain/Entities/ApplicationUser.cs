using Microsoft.AspNetCore.Identity;

namespace Library.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}