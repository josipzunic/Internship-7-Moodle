using Moodle.Domain.Entities.Notifications;

namespace Moodle.Application.Interfaces;

public interface INotificationRepository
{
    Task AddNotificationAsync(string title, string body, int courseId, int professorId);
}