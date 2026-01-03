using Moodle.Domain.Enums;

namespace Moodle.Application.Dto;

public class UserDto
{
    public int  Id { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}