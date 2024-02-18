using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Text;
using Synonms.RestEasy.Sample.Api.Employees;

namespace Synonms.RestEasy.Sample.Api.Contracts;

public class ContractResource : Resource
{
    public ContractResource()
    {
    }

    public ContractResource(Guid id, Link selfLink)
        : base(id, selfLink)
    {
    }

    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.Guid)]
    [RestEasyDescriptor(placeholder: Placeholders.Guid)]
    public EntityId<Employee> EmployeeId { get; set; } = EntityId<Employee>.Uninitialised;
    
    [RestEasyRequired]
    public DateOnly StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }

    [RestEasyOption("Permanent", "Permanent")]
    [RestEasyOption("Contract", "Contract")]
    [RestEasyOption("Zero Hours", "Zero Hours")]
    public string? EmploymentType { get; set; }
}