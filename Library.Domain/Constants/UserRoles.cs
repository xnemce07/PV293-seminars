namespace Library.Domain.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Librarian = "Librarian";
    public const string Member = "Member";

    public static readonly string[] AllRoles = { Admin, Librarian, Member };
}