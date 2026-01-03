using Moodle.Application.Dto;
using Moodle.Domain.Enums;

namespace Moodle.Application.Interfaces;

public interface IAdminRepository
{
    Task<List<UserDto>> GetAllUsers(int userId);
    Task DeleteUserAsync(int userId);
    Task<bool> UpdateUserEmailAsync(int userId, string newEmail);
    Task UpdateUserRoleAsync(int userId, Role role);
}