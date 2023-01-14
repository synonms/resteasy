using System.Linq.Expressions;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Sample.Api.Hacks;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressRepository : IReadRepository<Address>
{
    private static readonly List<Address> Addresses = new()
    {
        FunctionalHelper.FromResult(Address.Create(new AddressResource("Some Street", "Svartalfheim", "SV1 1SS"))).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource("Awful Avenue", "Alfheim", "AL2 2AA"))).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource("Manky Mews", "Midgard", "MI3 3MM"))).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003"))
    };
    
    public Task<Maybe<Address>> FindAsync(EntityId<Address> id)
    {
        Address? address = Addresses.SingleOrDefault(x => x.Id == id);
        Maybe<Address> outcome = address is null ? Maybe<Address>.None : Maybe<Address>.Some(address);

        return Task.FromResult(outcome);
    }

    public Task<IQueryable<Address>> QueryAsync(Expression<Func<Address, bool>> predicate) =>
        Task.FromResult(Addresses.Where(predicate.Compile()).AsQueryable());

    public Task<PaginatedList<Address>> ReadAsync(int offset, int limit) =>
        Task.FromResult(PaginatedList<Address>.Create(Addresses, offset, limit, 20));
}