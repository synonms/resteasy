using Synonms.RestEasy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Synonms.RestEasy.EntityFramework.Configurations;

public abstract class AggregateMemberEntityTypeConfiguration<TAggregateMember> : IEntityTypeConfiguration<TAggregateMember>
    where TAggregateMember : AggregateMember<TAggregateMember>
{
    public virtual void Configure(EntityTypeBuilder<TAggregateMember> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(_ => _.CreatedAt).IsRequired();
        builder.Ignore(x => x.DomainEvents);

        ConfigureAggregateMemberProperties(builder);
    }

    public abstract void ConfigureAggregateMemberProperties(EntityTypeBuilder<TAggregateMember> builder);
}