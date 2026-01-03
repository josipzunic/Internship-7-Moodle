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

    public async Task UpdateUserEmailAsync(int userId, string newEmail)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            Console.WriteLine("Korisnik nije pronađen");

        user.Email = newEmail;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
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