using Microsoft.EntityFrameworkCore;
using Moodle.Application.Dto;
using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    public CourseRepository(AppDbContext context) => _context = context; 

    public async Task<List<CourseOverviewDto>> GetCoursesAsync(int userId)
    {
        var result = await _context.Enrollments
            .Where(e => e.UserId == userId)
            .Select(e => e.CourseId)
            .Distinct()
            .Join(
                _context.Courses,
                courseId => courseId,
                course => course.Id,
                (courseId, course) => new CourseOverviewDto
                {
                    CourseId = course.Id,
                    CourseName = course.CourseName,

                    Notifications = _context.Notifications
                        .Where(n => n.CourseId == course.Id)
                        .Join(
                            _context.Users,
                            n => n.ProfessorId,
                            u => u.Id,
                            (n, u) => new NotificationDto
                            {
                                CreatedAt = n.CreatedAt,
                                ProfessorEmail = u.Email,
                                Title = n.Title,
                                Content = n.Content
                            })
                        .OrderByDescending(n => n.CreatedAt)
                        .ToList(),

                    Materials = _context.Materials
                        .Where(m => m.CourseId == course.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => new MaterialDto
                        {
                            CreatedAt = m.CreatedAt,
                            Name = m.Name,
                            Url = m.Url
                        })
                        .ToList()
                })
            .ToListAsync();

        return result;
    }
}