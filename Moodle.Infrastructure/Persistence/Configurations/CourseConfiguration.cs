using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moodle.Domain.Entities;

namespace Moodle.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.Property(x=> x.CourseName).HasMaxLength(100).IsRequired();
        builder.Property(x=> x.CourseDescription).HasMaxLength(500).IsRequired();
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ProfessorId);
        builder.Property(x=> x.CreatedAt).IsRequired();
        builder.Property(x=> x.UpdatedAt).IsRequired();
    }
}