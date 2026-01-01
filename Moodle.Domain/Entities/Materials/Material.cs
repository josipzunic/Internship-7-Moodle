namespace Moodle.Domain.Entities.Materials;

public class Material
{
    public int Id {get; set;}
    public int CourseId {get; set;}
    public int ProfessorId {get; set;}
    public string Url { get; set; } = null!;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}