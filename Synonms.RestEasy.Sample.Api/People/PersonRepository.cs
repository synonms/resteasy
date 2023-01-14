using System.Linq.Expressions;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Hacks;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonRepository : IReadRepository<Person>
{
    private static readonly List<Person> People = new()
    {
        FunctionalHelper.FromResult(Person.Create(new PersonResource("Kendrick", "Lamar", new DateOnly(1984, 5, 5), EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001")))).WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000001")),
        FunctionalHelper.FromResult(Person.Create(new PersonResource("Michael", "Archer", new DateOnly(1984, 6, 6), EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002")))).WithId(EntityId<Person>.Parse("00000000-0000-0000-0001-000000000002")),
        FunctionalHelper.FromResult(Person.Create(new PersonResource("Jeff", "Buckley", new DateOnly(1984, 7, 7), EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003")))).WithId(EntityId<Person>.Parse("00000000-0000-0000-0001-000000000003")),
    };
    
    public Task<Maybe<Person>> FindAsync(EntityId<Person> id)
    {
        Person? person = People.SingleOrDefault(x => x.Id == id);
        Maybe<Person> outcome = person is null ? Maybe<Person>.None : Maybe<Person>.Some(person);

        return Task.FromResult(outcome);
    }

    public Task<IQueryable<Person>> QueryAsync(Expression<Func<Person, bool>> predicate) =>
        Task.FromResult(People.Where(predicate.Compile()).AsQueryable());

    public Task<PaginatedList<Person>> ReadAsync(int offset, int limit) =>
        Task.FromResult(PaginatedList<Person>.Create(People, offset, limit, 20));
}