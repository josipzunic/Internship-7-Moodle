namespace Moodle.Domain.Entities.Notifications;

public class Notification
{
    public int Id { get; set; }
    public int CourseId {get; set;}
    public int ProfessorId {get; set;}
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}