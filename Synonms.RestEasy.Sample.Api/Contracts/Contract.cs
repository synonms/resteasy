using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Rules;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;
using Synonms.RestEasy.Core.Domain.ValueObjects;
using Synonms.RestEasy.Sample.Api.Employees;

namespace Synonms.RestEasy.Sample.Api.Contracts;

[RestEasyResource(typeof(ContractResource), "contracts", allowAnonymous: true)]
public class Contract : AggregateRoot<Contract>
{
    private Contract(EntityId<Contract> id, EntityId<Employee> employeeId, EventDate startDate, EventDate? endDate, EmploymentType? employmentType)
        : this(employeeId, startDate, endDate, employmentType)
    {
        Id = id;
    }
    
    private Contract(EntityId<Employee> employeeId, EventDate startDate, EventDate? endDate, EmploymentType? employmentType)
    {
        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        EmploymentType = employmentType;
    }
    
    public EntityId<Employee> EmployeeId { get; private set; }
    
    public EventDate? StartDate { get; private set; }
    
    public EventDate? EndDate { get; private set; }
    
    public EmploymentType? EmploymentType { get; private set; }
    
    public static Result<Contract> Create(ContractResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.StartDate, EventDate.CreateMandatory, out EventDate startDateValueObject)
            .WithOptionalValueObject(resource.EndDate, EventDate.CreateOptional, out EventDate? endDateValueObject)
            .WithOptionalValueObject(resource.EmploymentType, EmploymentType.CreateOptional, out EmploymentType? employmentTypeValueObject)
            .WithDomainRules(
                RelatedEntityIdRuleset<Employee>.Create(nameof(ContractResource.EmployeeId), resource.EmployeeId)
                )
            .Build()
            .ToResult(new Contract(
                (EntityId<Contract>)resource.Id, 
                resource.EmployeeId, 
                startDateValueObject, 
                endDateValueObject,
                employmentTypeValueObject
                ));

    public Maybe<Fault> Update(ContractResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.StartDate, EventDate.CreateMandatory, out EventDate startDateValueObject)
            .WithOptionalValueObject(resource.EndDate, EventDate.CreateOptional, out EventDate? endDateValueObject)
            .WithOptionalValueObject(resource.EmploymentType, EmploymentType.CreateOptional, out EmploymentType? employmentTypeValueObject)
            .Build()
            .BiBind(() =>
            {
                UpdateOptionalValue(_ => _.StartDate, startDateValueObject);
                UpdateOptionalValue(_ => _.EndDate, endDateValueObject);
                UpdateOptionalValue(_ => _.EmploymentType, employmentTypeValueObject);
                
                return Maybe<Fault>.None;
            });
}