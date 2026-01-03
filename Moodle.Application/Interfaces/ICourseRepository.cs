using Moodle.Application.Dto;
using Moodle.Domain.Entities;

namespace Moodle.Application.Interfaces;

public interface ICourseRepository
{
    Task<List<CourseOverviewDto>> GetCoursesAsync(int userId);
}