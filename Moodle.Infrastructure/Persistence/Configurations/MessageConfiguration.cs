using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moodle.Domain.Entities;
using Moodle.Domain.Entities.Messages;

namespace Moodle.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.SenderId);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReceiverId);
        builder.Property(x => x.SenderId).IsRequired();
        builder.Property(x => x.ReceiverId).IsRequired();
        builder.Property(x => x.Content).IsRequired().HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}