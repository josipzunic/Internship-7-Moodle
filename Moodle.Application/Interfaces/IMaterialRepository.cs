namespace Moodle.Application.Interfaces;

public interface IMaterialRepository
{
    Task AddMaterialAsync(string title, string url, int courseId, int professorId);
}