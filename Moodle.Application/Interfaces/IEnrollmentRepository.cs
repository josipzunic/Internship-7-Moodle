namespace Moodle.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task EnrollStudentAsync(int studentId, int courseId);
}