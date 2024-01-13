using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules;

public class DomainRulesPropertyBuilder<T>
{
    private readonly DomainRulesBuilder _builder;
    private readonly string _propertyName;
    private readonly T _value;
    internal readonly List<DomainRuleFault> Faults = new();

    public DomainRulesPropertyBuilder(DomainRulesBuilder builder, string propertyName, T value)
    {
        _builder = builder;
        _propertyName = propertyName;
        _value = value;
    }
    
    public DomainRulesPropertyBuilder<T> AddRule(Func<T, bool> isFaultPredicate, string faultDetail, params object?[] faultArguments)
    {
        if (isFaultPredicate(_value))
        {
            Faults.Add(new DomainRuleFault(faultDetail, new FaultSource(_propertyName, _value?.ToString()), faultArguments));
        }

        return this;
    }

    public DomainRulesPropertyBuilder<TNext> Property<TNext>(string propertyName, TNext value) =>
        _builder.Property(propertyName, value);

    public IEnumerable<DomainRuleFault> Build() => 
        _builder.Build();
}