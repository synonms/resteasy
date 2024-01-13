using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Text;
using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Resources;

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