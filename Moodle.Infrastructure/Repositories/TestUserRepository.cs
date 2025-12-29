using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;

namespace Moodle.Infrastructure.Repositories;

public class TestUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    
    public User? GetUserByEmail(string email)
    {
        return _users.SingleOrDefault(u => u.Email == email);
    }

    public void AddUser(User user)
    {
        _users.Add(user);
    }
}