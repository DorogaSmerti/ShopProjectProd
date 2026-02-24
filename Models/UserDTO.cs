namespace MyFirstProject.Models;

public class UsersDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
}

public class ChangeRoleDto
{
    public string UserName { get; set; }
    public string Role { get; set; }
}

public class DeleteUserDto
{
    public string UserId { get; set; }
}