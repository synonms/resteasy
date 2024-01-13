using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules.Rulesets;

public class ClosedDateOnlyRangeRuleset : IDomainRuleset
{
    private readonly string _startDatePropertyName;
    private readonly string _endDatePropertyName;
    private readonly DateOnly _startDate;
    private readonly DateOnly _endDate;
    
    private ClosedDateOnlyRangeRuleset(string startDatePropertyName, string endDatePropertyName, DateOnly startDate, DateOnly endDate)
    {
        _startDatePropertyName = startDatePropertyName;
        _endDatePropertyName = endDatePropertyName;
        _startDate = startDate;
        _endDate = endDate;
    }

    public IEnumerable<DomainRuleFault> Apply() =>
        DomainRules.CreateBuilder()
            .Property(_endDatePropertyName, _endDate)
            .AddRule(endDate => endDate < _startDate, ErrorTemplates.EndDateBeforeStartDateError, _endDate, _startDate)
            .Build();

    public static ClosedDateOnlyRangeRuleset Create(string startDatePropertyName, string endDatePropertyName, DateOnly startDate, DateOnly endDate) => 
        new(startDatePropertyName, endDatePropertyName, startDate, endDate);
}