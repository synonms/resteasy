using Synonms.RestEasy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Synonms.RestEasy.EntityFramework.Configurations;

public abstract class AggregateRootEntityTypeConfiguration<TAggregateRoot> : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public virtual void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        builder.HasKey(aggregateRoot => aggregateRoot.Id);
        builder.Property(aggregateRoot => aggregateRoot.Id).IsRequired();
        builder.Property(aggregateRoot => aggregateRoot.IsActive).IsRequired();
        builder.Property(aggregateRoot => aggregateRoot.CreatedAt).IsRequired();
        builder.Property(aggregateRoot => aggregateRoot.UpdatedAt);
        builder.Property(aggregateRoot => aggregateRoot.EntityTag).IsRequired().IsConcurrencyToken();
        builder.Ignore(aggregateRoot => aggregateRoot.DomainEvents);

        ConfigureAggregateProperties(builder);
    }

    public abstract void ConfigureAggregateProperties(EntityTypeBuilder<TAggregateRoot> builder);
}