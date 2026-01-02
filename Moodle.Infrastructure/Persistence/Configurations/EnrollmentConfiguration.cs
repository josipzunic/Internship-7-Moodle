using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moodle.Domain.Entities;
using Moodle.Domain.Entities.Enrollments;

namespace Moodle.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollement>
{
    public void Configure(EntityTypeBuilder<Enrollement> builder)
    {
        builder.ToTable("Enrollments");
        
        builder.HasKey(x=>x.Id);
        builder.Property(x=>x.Id);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne<Course>().WithMany().HasForeignKey(x => x.CourseId);
        builder.Property(x => x.CourseId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x=>x.CreatedAt).IsRequired();
        builder.Property(x=>x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
    }
}