using Moodle.Domain.Entities;

namespace Moodle.Application.Interfaces;

public interface IUserRepository
{
    Task<User?>  GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
}