using Synonms.RestEasy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Synonms.RestEasy.EntityFramework.Configurations;

public abstract class AggregateMemberEntityTypeConfiguration<TAggregateMember> : IEntityTypeConfiguration<TAggregateMember>
    where TAggregateMember : AggregateMember<TAggregateMember>
{
    public virtual void Configure(EntityTypeBuilder<TAggregateMember> builder)
    {
        builder.HasKey(aggregateMember => aggregateMember.Id);
        builder.Property(aggregateMember => aggregateMember.Id).IsRequired();
        builder.Property(aggregateRoot => aggregateRoot.IsActive).IsRequired();
        builder.Property(aggregateMember => aggregateMember.CreatedAt).IsRequired();
        builder.Property(aggregateMember => aggregateMember.UpdatedAt);
        builder.Ignore(aggregateMember => aggregateMember.DomainEvents);

        ConfigureAggregateMemberProperties(builder);
    }

    public abstract void ConfigureAggregateMemberProperties(EntityTypeBuilder<TAggregateMember> builder);
}