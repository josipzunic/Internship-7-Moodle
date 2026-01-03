using Moodle.Application.Interfaces;
using Moodle.Domain.Entities.Notifications;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    public NotificationRepository(AppDbContext context) => _context = context; 
        
    public async Task AddNotificationAsync(string title, string body, int courseId, int professorId)
    {
        var notification = new Notification()
        {
            Title = title,
            Content = body,
            CourseId = courseId,
            ProfessorId = professorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}