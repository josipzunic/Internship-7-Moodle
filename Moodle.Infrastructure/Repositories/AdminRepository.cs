using Microsoft.EntityFrameworkCore;
using Moodle.Application.Dto;
using Moodle.Application.Interfaces;
using Moodle.Domain.Enums;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;
    public AdminRepository(AppDbContext context) => _context = context; 
    
    public async Task<List<UserDto>> GetAllUsers(int userId)
    {
        return await _context.Users.Where(u => u.Id != userId).Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role
        })
        .ToListAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return; 
        
        var messages = await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .ToListAsync();

        _context.Messages.RemoveRange(messages);
        
        var enrollments = await _context.Enrollments
            .Where(e => e.UserId == userId)
            .ToListAsync();

        _context.Enrollments.RemoveRange(enrollments);
        
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateUserEmailAsync(int userId, string newEmail)
    {
        bool success = false;
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            Console.WriteLine("Korisnik nije pronađen");

        if (user != null &&
            !string.IsNullOrWhiteSpace(newEmail) &&
            newEmail.Contains("@") &&
            newEmail.Split('@')[0].Length >= 1)
        {
            var parts = newEmail.Split('@');
            var domainParts = parts[1].Split('.');
    
            if (domainParts.Length == 2 &&
                domainParts[0].Length >= 2 &&
                domainParts[1].Length >= 3)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newEmail);
                if (existingUser == null || existingUser.Id == user.Id)
                {
                    user.Email = newEmail;
                    user.UpdatedAt = DateTime.UtcNow;
                    success =  true;
                }
                else
                    Console.WriteLine("Greška: korisnik s tom email adresom već postoji.");
                
            }
            else
                Console.WriteLine("Domena emaila mora biti u formatu [domena].[tld] (npr. gmail.com).");
        }
        else
            Console.WriteLine("Email nije validan ili korisnik nije pronađen.");
        
        await _context.SaveChangesAsync();
        return success;
    }

    public async Task UpdateUserRoleAsync(int userId, Role role)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
            Console.WriteLine("Korisnik nije pronađen");

        Console.WriteLine(role.ToString());
        if (role == Role.Professor)
            user.Role = Role.Student;
        else 
            user.Role = Role.Professor;
        
        await _context.SaveChangesAsync();

    }
}