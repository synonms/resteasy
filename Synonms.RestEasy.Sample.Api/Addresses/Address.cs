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
    
    private Address(EntityId<Address> id, RestEasyAddressLine line1, RestEasyAddressLine line2, RestEasyPostCode postCode)
        : this(line1, line2, postCode)
    {
        Id = id;
    }
    
    private Address(RestEasyAddressLine line1, RestEasyAddressLine line2, RestEasyPostCode postCode)
    {
        Line1 = line1;
        Line2 = line2;
        PostCode = postCode;
    }
    
    public RestEasyAddressLine Line1 { get; private set; }
    
    public RestEasyAddressLine Line2 { get; private set; }
    
    public RestEasyPostCode PostCode { get; private set; }

    public static Result<Address> Create(AddressResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Line1, x => RestEasyAddressLine.CreateMandatory(x, AddressLineMaxLength), out RestEasyAddressLine line1ValueObject)
            .WithMandatoryValueObject(resource.Line2, x => RestEasyAddressLine.CreateMandatory(x, AddressLineMaxLength), out RestEasyAddressLine line2ValueObject)
            .WithMandatoryValueObject(resource.PostCode, x => RestEasyPostCode.CreateMandatory(x, PostCodeMaxLength), out RestEasyPostCode postCodeValueObject)
            .Build()
            .ToResult(new Address(line1ValueObject, line2ValueObject, postCodeValueObject));

    public Maybe<Fault> Update(AddressResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Line1, x => RestEasyAddressLine.CreateMandatory(x, AddressLineMaxLength), out RestEasyAddressLine line1ValueObject)
            .WithMandatoryValueObject(resource.Line2, x => RestEasyAddressLine.CreateMandatory(x, AddressLineMaxLength), out RestEasyAddressLine line2ValueObject)
            .WithMandatoryValueObject(resource.PostCode, x => RestEasyPostCode.CreateMandatory(x, PostCodeMaxLength), out RestEasyPostCode postCodeValueObject)
            .Build()
            .BiBind(() =>
            {
                Line1 = line1ValueObject;
                Line2 = line2ValueObject;
                PostCode = postCodeValueObject;

                return Maybe<Fault>.None;
            });
}