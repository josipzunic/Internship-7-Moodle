using Moodle.Application.Interfaces;
using Moodle.Domain.Entities.Materials;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;
    public MaterialRepository(AppDbContext context) => _context = context; 
    
    public async Task AddMaterialAsync(string title, string url, int  courseId, int professorId)
    {
        var material = new Material()
        {
            Name = title,
            Url = url,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CourseId = courseId,
            ProfessorId = professorId
        };
        
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();
    }
}