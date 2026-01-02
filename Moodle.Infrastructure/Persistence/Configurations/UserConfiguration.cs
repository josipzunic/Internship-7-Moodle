using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moodle.Domain.Entities;

namespace Moodle.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Role)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired();
        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime")
            .IsRequired();
    }
}