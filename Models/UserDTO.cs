namespace MyFirstProject.Models;

public class UsersDTO
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required IList<string> Roles { get; set; }
}

public class ChangeRoleDto
{
    public required string UserName { get; set; }
    public required string Role { get; set; }
}

public class DeleteUserDto
{
    public required string UserId { get; set; }
}