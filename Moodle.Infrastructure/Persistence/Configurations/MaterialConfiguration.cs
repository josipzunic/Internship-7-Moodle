using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moodle.Domain.Entities;
using Moodle.Domain.Entities.Materials;

namespace Moodle.Infrastructure.Persistence.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.ProfessorId).IsRequired();
        builder.Property(x => x.CourseId).IsRequired();
        builder.HasOne<Course>().WithMany().HasForeignKey(x => x.CourseId);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ProfessorId);
    }
}