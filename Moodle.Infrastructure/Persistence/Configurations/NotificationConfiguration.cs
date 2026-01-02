using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moodle.Domain.Entities;
using Moodle.Domain.Entities.Notifications;

namespace Moodle.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.CourseId).IsRequired();
        builder.Property(x => x.ProfessorId).IsRequired();
        builder.HasOne<Course>().WithMany().HasForeignKey(x => x.CourseId);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ProfessorId);
    }
}