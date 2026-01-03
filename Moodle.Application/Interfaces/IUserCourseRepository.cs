using Moodle.Application.Dto;
using Moodle.Domain.Entities;

namespace Moodle.Application.Interfaces;

public interface IUserCourseRepository
{
    Task<List<CourseOverviewDto>> GetCoursesAsync(int userId);
    Task<List<CourseOverviewDto>> GetCoursesOverviewForProfessorAsync(int userId);
    Task<List<StudentDto>> GetStudentsNotEnrolledInCourseAsync(int courseId);
}