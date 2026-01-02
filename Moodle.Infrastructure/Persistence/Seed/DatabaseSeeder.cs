using Microsoft.EntityFrameworkCore;
using Moodle.Domain.Entities;
using Moodle.Domain.Entities.Enrollments;
using Moodle.Domain.Entities.Materials;
using Moodle.Domain.Entities.Messages;
using Moodle.Domain.Entities.Notifications;
using Moodle.Domain.Enums;
using Moodle.Domain.Utilities;

namespace Moodle.Infrastructure.Persistence;

public class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.Migrate();

        if (context.Users.Any()) return;

        var admin = new User
        {
            Email = "admin@moodle.com",
            PasswordHash = PasswordHasher.Hash("admin123"),
            Role = Role.Admin,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var professor1 = new User
        {
            Email = "professor1@moodle.com",
            PasswordHash = PasswordHasher.Hash("professor1-123"),
            Role = Role.Professor,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var professor2 = new User
        {
            Email = "professor2@moodle.com",
            PasswordHash = PasswordHasher.Hash("professor2-123"),
            Role = Role.Professor,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var student1 = new User
        {
            Email = "student1@moodle.com",
            PasswordHash = PasswordHasher.Hash("student1-123"),
            Role = Role.Student,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var student2 = new User
        {
            Email = "student2@moodle.com",
            PasswordHash = PasswordHasher.Hash("student2-123"),
            Role = Role.Student,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Users.AddRange(admin, professor1, student1, student2, professor2);
        context.SaveChanges();

        var course1 = new Course
        {
            CourseDescription = "Newtonova mehanika",
            CourseName = "Klasicna mehanika 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProfessorId = professor1.Id
        };

        var course2 = new Course
        {
            CourseDescription = "Lagrangeova  mehanika",
            CourseName = "Klasicna mehanika 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProfessorId = professor2.Id
        };
        
        context.Courses.AddRange(course1, course2);
        context.SaveChanges();

        var enrollment1 = new Enrollement
        {
            CourseId = course1.Id,
            UserId = student1.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var enrollment2 = new Enrollement
        {
            CourseId = course2.Id,
            UserId = student2.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Enrollments.AddRange(enrollment1, enrollment2);
        context.SaveChanges();

        var material1 = new Material
        {
            CourseId = course1.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Material 1",
            Url = "Klasicna mehanika 1",
            ProfessorId = professor1.Id
        };

        var material2 = new Material
        {
            CourseId = course2.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Material 2",
            Url = "Klasicna mehanika 2",
            ProfessorId = professor2.Id
        };
        
        context.Materials.AddRange(material1, material2);
        context.SaveChanges();

        var message1 = new Message
        {
            Content = "When is class starting",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SenderId = student1.Id,
            ReceiverId = professor1.Id,
        };

        var message2 = new Message
        {
            Content = "14:15",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SenderId = professor1.Id,
            ReceiverId = student1.Id,
        };
        
        context.Messages.AddRange(message1, message2);
        context.SaveChanges();

        var notification1 = new Notification
        {
            Content = "21/10/2026",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CourseId = course2.Id,
            ProfessorId = professor2.Id,
            Title = "Exam information"
        };
        
        var notification2 = new Notification
        {
            Content = "Watch found in B-101",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CourseId = course2.Id,
            ProfessorId = professor2.Id,
            Title = "Lost item"
        };
        
        context.Notifications.AddRange(notification1, notification2);
        context.SaveChanges();
    }
}