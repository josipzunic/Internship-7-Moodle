using Moodle.Domain.Entities;

namespace Moodle.Application.Interfaces;

public interface IUserRepository
{
    User?  GetUserByEmail(string email);
    void AddUser(User user);
}