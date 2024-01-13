using Synonms.RestEasy.Core.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synonms.RestEasy.EntityFramework.Configurations;

namespace Synonms.RestEasy.EntityFramework.Persistence.Migrations;

public class MigrationConfiguration : AggregateRootEntityTypeConfiguration<Migration>
{
    public override void ConfigureAggregateProperties(EntityTypeBuilder<Migration> builder)
    {
        builder.ToTable("Migrations", FrameworkDatabase.Schema);
        
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ToVersion).IsRequired();
    }
}