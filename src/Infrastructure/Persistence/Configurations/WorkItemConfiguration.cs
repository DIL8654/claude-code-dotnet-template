using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("WorkItems");
        builder.HasKey(workItem => workItem.Id);

        builder.Property(workItem => workItem.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(workItem => workItem.Description)
            .HasMaxLength(2000);

        builder.Property(workItem => workItem.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(workItem => workItem.RowVersion)
            .IsRequired()
            .IsConcurrencyToken();

        builder.HasIndex(workItem => new { workItem.Status, workItem.CreatedUtc });
    }
}
