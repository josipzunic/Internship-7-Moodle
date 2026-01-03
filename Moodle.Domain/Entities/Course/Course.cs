namespace Moodle.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string CourseName { get; set; } = null!;
    public string CourseDescription { get; set; } = null!;
    public int ProfessorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}