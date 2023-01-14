using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.ValueObjects;

namespace Synonms.RestEasy.Sample.Api.Addresses;

[RestEasyResource("addresses")]
public class Address : AggregateRoot<Address>
{
    public const int AddressLineMaxLength = 40;
    public const int PostCodeMaxLength = 10;
    
    private Address(EntityId<Address> id, AddressLine line1, AddressLine line2, PostCode postCode)
        : this(line1, line2, postCode)
    {
        Id = id;
    }
    
    private Address(AddressLine line1, AddressLine line2, PostCode postCode)
    {
        Line1 = line1;
        Line2 = line2;
        PostCode = postCode;
    }
    
    public AddressLine Line1 { get; private set; }
    
    public AddressLine Line2 { get; private set; }
    
    public PostCode PostCode { get; private set; }

    public static Result<Address> Create(AddressResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Line1, x => AddressLine.CreateMandatory(x, AddressLineMaxLength), out AddressLine line1ValueObject)
            .WithMandatoryValueObject(resource.Line2, x => AddressLine.CreateMandatory(x, AddressLineMaxLength), out AddressLine line2ValueObject)
            .WithMandatoryValueObject(resource.PostCode, x => PostCode.CreateMandatory(x, PostCodeMaxLength), out PostCode postCodeValueObject)
            .Build()
            .ToResult(new Address(line1ValueObject, line2ValueObject, postCodeValueObject));

    public Maybe<Fault> Update(AddressResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Line1, x => AddressLine.CreateMandatory(x, AddressLineMaxLength), out AddressLine line1ValueObject)
            .WithMandatoryValueObject(resource.Line2, x => AddressLine.CreateMandatory(x, AddressLineMaxLength), out AddressLine line2ValueObject)
            .WithMandatoryValueObject(resource.PostCode, x => PostCode.CreateMandatory(x, PostCodeMaxLength), out PostCode postCodeValueObject)
            .Build()
            .BiBind(() =>
            {
                Line1 = line1ValueObject;
                Line2 = line2ValueObject;
                PostCode = postCodeValueObject;

                return Maybe<Fault>.None;
            });
}