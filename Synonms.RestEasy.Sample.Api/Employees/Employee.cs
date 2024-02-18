using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Rules;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;
using Synonms.RestEasy.Core.Domain.ValueObjects;
using Synonms.RestEasy.Sample.Api.Addresses;

namespace Synonms.RestEasy.Sample.Api.Employees;

[RestEasyResource(typeof(EmployeeResource), "employees", allowAnonymous: true, pageLimit: 5)]
public class Employee : AggregateRoot<Employee>
{
    public const int InitialsMaxLength = Initials.DefaultMaxLength;
    public const int ForenameMaxLength = 50;
    public const int MiddleNamesMaxLength = 50;
    public const int SurnameMaxLength = 50;
    
    private Employee(EntityId<Employee> id, Title? title, Initials? initials, Moniker forename, Moniker? middleNames, Moniker surname, Moniker? legalForename, Moniker? legalSurname, Sex sex, EventDate dateOfBirth, EmploymentDetails employmentDetails, EntityId<Address> homeAddressId)
        : this(title, initials, forename, middleNames, surname, legalForename, legalSurname, sex, dateOfBirth, employmentDetails, homeAddressId)
    {
        Id = id;
    }
    
    private Employee(Title? title, Initials? initials, Moniker forename, Moniker? middleNames, Moniker surname, Moniker? legalForename, Moniker? legalSurname, Sex sex, EventDate dateOfBirth, EmploymentDetails employmentDetails, EntityId<Address> homeAddressId)
    {
        Title = title;
        Initials = initials;
        Forename = forename;
        MiddleNames = middleNames;
        Surname = surname;
        LegalForename = legalForename;
        LegalSurname = legalSurname;
        Sex = sex;
        DateOfBirth = dateOfBirth;
        EmploymentDetails = employmentDetails;
        HomeAddressId = homeAddressId;
    }
    
    public Title? Title { get; private set; }
    
    public Initials? Initials { get; private set; }
    
    public Moniker Forename { get; private set; }
    
    public Moniker? MiddleNames { get; private set; }

    public Moniker Surname { get; private set; }

    public Moniker? LegalForename { get; private set; }
    
    public Moniker? LegalSurname { get; private set; }

    public Sex Sex { get; private set; }
    
    public EventDate DateOfBirth { get; private set; }
    
    public EmploymentDetails EmploymentDetails { get; private set; }

    public EntityId<Address> HomeAddressId { get; private set; }
    
    public static Result<Employee> Create(EmployeeResource resource) =>
        EmploymentDetails.Create((EntityId<Employee>)resource.Id, resource.EmploymentDetails)
            .Bind(employmentDetails =>
                AggregateRules.CreateBuilder()
                    .WithOptionalValueObject(resource.Title, Title.CreateOptional, out Title? titleValueObject)
                    .WithOptionalValueObject(resource.Initials, Initials.CreateOptional, out Initials? initialsValueObject)
                    .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
                    .WithOptionalValueObject(resource.MiddleNames, x => Moniker.CreateOptional(x, MiddleNamesMaxLength), out Moniker? middleNamesValueObject)
                    .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
                    .WithOptionalValueObject(resource.LegalForename, x => Moniker.CreateOptional(x, ForenameMaxLength), out Moniker? legalForenameValueObject)
                    .WithOptionalValueObject(resource.LegalSurname, x => Moniker.CreateOptional(x, SurnameMaxLength), out Moniker? legalSurnameValueObject)
                    .WithMandatoryValueObject(resource.Sex, Sex.CreateMandatory, out Sex sexValueObject)
                    .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
                    .WithDomainRules(
                        RelatedEntityIdRuleset<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                    )
                    .Build()
                    .ToResult(new Employee((EntityId<Employee>)resource.Id, titleValueObject, initialsValueObject, forenameValueObject, middleNamesValueObject, surnameValueObject, legalForenameValueObject,
                        legalSurnameValueObject, sexValueObject, dateOfBirthValueObject, employmentDetails, resource.HomeAddressId))
            );

    public Maybe<Fault> Update(EmployeeResource resource) =>
        EmploymentDetails.Update(resource.EmploymentDetails, MarkAsUpdated)
            .BiBind(() =>
                AggregateRules.CreateBuilder()
                    .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
                    .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
                    .WithMandatoryValueObject(resource.Sex, Sex.CreateMandatory, out Sex sexValueObject)
                    .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
                    .WithOptionalValueObject(resource.Title, Title.CreateOptional, out Title? titleValueObject)
                    .WithOptionalValueObject(resource.Initials, Initials.CreateOptional, out Initials? initialsValueObject)
                    .WithOptionalValueObject(resource.MiddleNames, x => Moniker.CreateOptional(x, MiddleNamesMaxLength), out Moniker? middleNamesValueObject)
                    .WithOptionalValueObject(resource.LegalForename, x => Moniker.CreateOptional(x, ForenameMaxLength), out Moniker? legalForenameValueObject)
                    .WithOptionalValueObject(resource.LegalSurname, x => Moniker.CreateOptional(x, SurnameMaxLength), out Moniker? legalSurnameValueObject)
                    .WithDomainRules(
                        RelatedEntityIdRuleset<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                    )
                    .Build()
                    .BiBind(() =>
                    {
                        UpdateMandatoryValue(_ => _.Forename, forenameValueObject);
                        UpdateMandatoryValue(_ => _.Surname, surnameValueObject);
                        UpdateMandatoryValue(_ => _.Sex, sexValueObject);
                        UpdateMandatoryValue(_ => _.DateOfBirth, dateOfBirthValueObject);
                        UpdateMandatoryValue(_ => _.HomeAddressId, resource.HomeAddressId);
                        UpdateOptionalValue(_ => _.Title, titleValueObject);
                        UpdateOptionalValue(_ => _.Initials, initialsValueObject);
                        UpdateOptionalValue(_ => _.MiddleNames, middleNamesValueObject);
                        UpdateOptionalValue(_ => _.LegalForename, legalForenameValueObject);
                        UpdateOptionalValue(_ => _.LegalSurname, legalSurnameValueObject);

                        return Maybe<Fault>.None;
                    })
            );
}