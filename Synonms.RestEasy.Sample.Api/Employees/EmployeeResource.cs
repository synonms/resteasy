using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Text;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Contracts;

namespace Synonms.RestEasy.Sample.Api.Employees;

public class EmployeeResource : Resource
{
    public EmployeeResource()
    {
    }

    public EmployeeResource(Guid id, Link selfLink)
        : base(id, selfLink)
    {
    }

    [RestEasyLookup("Title")]
    public string? Title { get; set; }

    [RestEasyMaxLength(Employee.InitialsMaxLength)]
    public string? Initials { get; set; }

    [RestEasyRequired]
    [RestEasyMaxLength(Employee.ForenameMaxLength)]
    public string Forename { get; set; } = string.Empty;

    [RestEasyMaxLength(Employee.MiddleNamesMaxLength)]
    public string? MiddleNames { get; set; }

    [RestEasyRequired]
    [RestEasyMaxLength(Employee.SurnameMaxLength)]
    public string Surname { get; set; } = string.Empty;

    [RestEasyMaxLength(Employee.ForenameMaxLength)]
    public string? LegalForename { get; set; }

    [RestEasyMaxLength(Employee.SurnameMaxLength)]
    public string? LegalSurname { get; set; }

    [RestEasyRequired]
    [RestEasyLookup("Sex")]
    public string SexAssignedAtBirth { get; set; } = string.Empty;

    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.DateOnly)]
    [RestEasyDescriptor(placeholder: Placeholders.DateOnly)]
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;

    [RestEasyRequired] 
    public EmploymentDetailsResource EmploymentDetails { get; set; } = new();    
    
    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.Guid)]
    [RestEasyDescriptor(placeholder: Placeholders.Guid)]
    public EntityId<Address> HomeAddressId { get; set; } = EntityId<Address>.Uninitialised;
    
    public IEnumerable<EntityId<Contract>> Contracts { get; set; } = Enumerable.Empty<EntityId<Contract>>();
}