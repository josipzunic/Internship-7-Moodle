using Microsoft.EntityFrameworkCore;
using Moodle.Application.Dto;
using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Domain.Enums;
using Moodle.Infrastructure.Persistence;

namespace Moodle.Infrastructure.Repositories;

public class UserCourseRepository : IUserCourseRepository
{
    private readonly AppDbContext _context;
    public UserCourseRepository(AppDbContext context) => _context = context; 

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
    
    public async Task<List<CourseOverviewDto>> GetCoursesOverviewForProfessorAsync(int professorId)
    {
        var result = await _context.Courses
            .Where(c => c.ProfessorId == professorId)
            .Select(course => new CourseOverviewDto()
            {
                CourseId = course.Id,
                CourseName = course.CourseName,
                
                Students = _context.Enrollments
                    .Where(e => e.CourseId == course.Id)
                    .Join(
                        _context.Users,
                        e => e.UserId,
                        u => u.Id,
                        (e, u) => new StudentDto
                        {
                            Id = u.Id,
                            Email = u.Email
                        })
                    .OrderBy(s => s.Email)
                    .ToList(),
                
                Notifications = _context.Notifications
                    .Where(n => n.CourseId == course.Id)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new NotificationDto
                    {
                        CreatedAt = n.CreatedAt,
                        Title = n.Title,
                        Content = n.Content,
                        ProfessorEmail = _context.Users
                            .Where(u => u.Id == n.ProfessorId)
                            .Select(u => u.Email)
                            .First()
                    })
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

    public async Task<List<StudentDto>> GetStudentsNotEnrolledInCourseAsync(int courseId)
    {
        var students = await _context.Users
            .Where(u => u.Role == Role.Student)
            .Where(u => !_context.Enrollments
                .Any(e => e.UserId == u.Id && e.CourseId == courseId))
            .OrderBy(u => u.Email)
            .Select(u => new StudentDto
            {
                Id = u.Id,
                Email = u.Email
            })
            .ToListAsync();

        return students;
    }
}