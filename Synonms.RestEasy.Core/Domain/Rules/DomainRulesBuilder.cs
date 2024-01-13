using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules;

public class DomainRulesBuilder
{
    private readonly List<Func<IEnumerable<DomainRuleFault>>> _propertyBuildFuncs = new();
        
    public DomainRulesPropertyBuilder<T> Property<T>(string propertyName, T value)
    {
        DomainRulesPropertyBuilder<T> propertyBuilder = new (this, propertyName, value);
        _propertyBuildFuncs.Add(() => propertyBuilder.Faults);

        return propertyBuilder;
    }

    public IEnumerable<DomainRuleFault> Build() => 
        _propertyBuildFuncs.SelectMany(x => x.Invoke());
}