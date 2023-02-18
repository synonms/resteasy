using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.People;
using Synonms.RestEasy.Sample.Api.Pets;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryRepositories(this IServiceCollection serviceCollection)
    {
        InMemoryRepository<Person> peopleRepository = new(SeedData.People);
        
        serviceCollection.AddSingleton<ICreateRepository<Person>>(peopleRepository);
        serviceCollection.AddSingleton<IReadRepository<Person>>(peopleRepository);
        serviceCollection.AddSingleton<IUpdateRepository<Person>>(peopleRepository);
        serviceCollection.AddSingleton<IDeleteRepository<Person>>(peopleRepository);

        InMemoryRepository<Address> addressesRepository = new(SeedData.Addresses);

        serviceCollection.AddSingleton<ICreateRepository<Address>>(addressesRepository);
        serviceCollection.AddSingleton<IReadRepository<Address>>(addressesRepository);
        serviceCollection.AddSingleton<IUpdateRepository<Address>>(addressesRepository);
        serviceCollection.AddSingleton<IDeleteRepository<Address>>(addressesRepository);

        InMemoryRepository<Pet> petsRepository = new(SeedData.Pets);

        serviceCollection.AddSingleton<ICreateRepository<Pet>>(petsRepository);
        serviceCollection.AddSingleton<IReadRepository<Pet>>(petsRepository);
        serviceCollection.AddSingleton<IUpdateRepository<Pet>>(petsRepository);
        serviceCollection.AddSingleton<IDeleteRepository<Pet>>(petsRepository);

        return serviceCollection;
    }
}