using Synonms.RestEasy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Synonms.RestEasy.EntityFramework.Configurations;

public abstract class AggregateRootEntityTypeConfiguration<TAggregateRoot> : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public virtual void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        builder.HasKey(_ => _.Id);
        builder.Property(_ => _.Id).IsRequired();
        builder.Property(_ => _.CreatedAt).IsRequired();
        builder.Property(_ => _.EntityTag).IsRequired().IsConcurrencyToken();
        builder.Ignore(_ => _.DomainEvents);

        ConfigureAggregateProperties(builder);
    }

    public abstract void ConfigureAggregateProperties(EntityTypeBuilder<TAggregateRoot> builder);
}