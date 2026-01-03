namespace Moodle.Application.Dto;

public class NotificationDto
{
    public DateTime CreatedAt { get; set; }
    public string ProfessorEmail { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}
