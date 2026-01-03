using Microsoft.EntityFrameworkCore;
using Moodle.Application.Dto;
using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Domain.Entities.Messages;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class PrivateChatRepository : IPrivateChatRepository
{
    private readonly AppDbContext _context;
    public PrivateChatRepository(AppDbContext context) => _context = context; 
    
    public async Task<List<UserDto>> GetUsersWithoutConversationAsync(int userId)
    {
        var users = await _context.Users
            .Where(u => u.Id != userId)
            .Where(u => !_context.Messages
                .Any(m =>
                    (m.SenderId == userId && m.ReceiverId == u.Id) ||
                    (m.SenderId == u.Id && m.ReceiverId == userId)
                )
            )
            .OrderBy(u => u.Email)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        return users;
    }
    
    public async Task OpenPrivateChatAsync(int senderId, int recieverId)
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            
            var messages = await _context.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == recieverId) ||
                            (m.SenderId == recieverId && m.ReceiverId == senderId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
            
            foreach (var msg in messages)
            {
                var sender = msg.SenderId == senderId ? "You" : (await _context.Users.FindAsync(msg.SenderId))!.Email;
                Console.WriteLine($"[{msg.CreatedAt:HH:mm}] {sender}: {msg.Content}");
            }
            
            Console.WriteLine("\nNova poruka (/exit za izaći):");
            var input = Console.ReadLine()!.Trim();

            if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
            {
                exit = true;
                continue;
            }
            
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = recieverId,
                Content = input,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UserDto>> GetUsersWithConversationAsync(int userId)
    {
        var users = await _context.Users
            .Where(u => u.Id != userId)
            .Where(u => _context.Messages
                .Any(m =>
                    (m.SenderId == userId && m.ReceiverId == u.Id) ||
                    (m.SenderId == u.Id && m.ReceiverId == userId)
                )
            )
            .OrderBy(u => u.Email)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        return users;
    }
}