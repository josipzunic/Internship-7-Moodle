using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Domain.Enums;

namespace Moodle.Application.Services;

public class AuthentificationService
{
    private readonly IUserRepository _userRepository;
    
    public AuthentificationService(IUserRepository userRepository) => _userRepository = userRepository;

    public bool RegisterUser(string email, string password)
    {
        if (_userRepository.GetUserByEmail(email) != null) return false;

        var user = new User
        {
            Email = email,
            Role = Role.Student,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        user.SetPasswordHash(password);
        _userRepository.AddUser(user);
        
        return true;
    }

    public User? LoginUser(string email, string password)
    {
        var user = _userRepository.GetUserByEmail(email);
        if(user == null) return null;
        
        return user.VerifyPassword(password) ?  user : null;
    }
}