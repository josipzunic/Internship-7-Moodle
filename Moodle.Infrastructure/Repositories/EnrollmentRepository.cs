using Microsoft.EntityFrameworkCore;
using Moodle.Application.Interfaces;
using Moodle.Domain.Entities.Enrollments;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;
    public EnrollmentRepository(AppDbContext context) => _context = context;
    
    public async Task EnrollStudentAsync(int studentId, int courseId)
    {
        var enrollmentExists = await _context.Enrollments
            .AnyAsync(e => e.UserId == studentId && e.CourseId == courseId);

        if (enrollmentExists)
            return;

        var enrollment = new Enrollement
        {
            UserId = studentId,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }
}