using Moodle.Domain.Enums;
using Moodle.Domain.Utilities;

namespace Moodle.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Role Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    
    public void SetPasswordHash(string password)
    {
        PasswordHash = PasswordHasher.Hash(password);
    }

    public bool VerifyPassword(string password) =>  PasswordHasher.Verify(password, PasswordHash);
}