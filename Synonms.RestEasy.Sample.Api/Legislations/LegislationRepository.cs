using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Legislations;

public class LegislationRepository : InMemoryRepository<Legislation>
{
    public LegislationRepository() : base(SeedData.Legislations)
    {
    }
}