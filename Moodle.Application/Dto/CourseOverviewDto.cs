namespace Moodle.Application.Dto;

public class CourseOverviewDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public List<NotificationDto> Notifications { get; set; } = new();
    public List<MaterialDto> Materials { get; set; } = new();
    public List<StudentDto> Students { get; set; } = new();
}