using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Tests.Unit.Domain;

public class AggregateRulesBuilderTests
{
    private record TestIntValueObject : ValueObject<int>, IComparable, IComparable<TestIntValueObject>
    {
        private TestIntValueObject(int value) : base(value)
        {
        }
    
        public static implicit operator int(TestIntValueObject intValueObject) => intValueObject.Value;

        public static OneOf<TestIntValueObject, IEnumerable<DomainRuleFault>> CreateMandatory(int value) => 
            new TestIntValueObject(value);

        public static OneOf<Maybe<TestIntValueObject>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) => 
            value is null ? Maybe<TestIntValueObject>.None : new TestIntValueObject(value.Value);
    
        public int CompareTo(TestIntValueObject? other) => Value.CompareTo(other);
    
        public int CompareTo(object? obj) => Value.CompareTo(obj);
    }

    private record TestStringValueObject : ValueObject<string>, IComparable, IComparable<TestStringValueObject>
    {
        private TestStringValueObject(string value) : base(value)
        {
        }
    
        public static implicit operator string(TestStringValueObject stringValueObject) => stringValueObject.Value;

        public static OneOf<TestStringValueObject, IEnumerable<DomainRuleFault>> CreateMandatory(string value) => 
            new TestStringValueObject(value);

        public static OneOf<Maybe<TestStringValueObject>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) => 
            value is null ? Maybe<TestStringValueObject>.None : new TestStringValueObject(value);

        public int CompareTo(TestStringValueObject? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
        public int CompareTo(object? obj) => Value.CompareTo(obj);
    }

    [Fact]
    public void WithOptionalValueObject_GivenNullValueType_ReturnsNullValueObject()
    {
        int? value = null;

        Maybe<Fault> outcome = AggregateRules.CreateBuilder()
            .WithOptionalValueObject(value, TestIntValueObject.CreateOptional, out TestIntValueObject? testValueObject)
            .Build();

        Assert.True(outcome.IsNone);
        Assert.Null(testValueObject);
    }
    
    [Fact]
    public void WithOptionalValueObject_GivenNullReferenceType_ReturnsNullValueObject()
    {
        string? value = null;

        Maybe<Fault> outcome = AggregateRules.CreateBuilder()
            .WithOptionalValueObject(value, TestStringValueObject.CreateOptional, out TestStringValueObject? testValueObject)
            .Build();

        Assert.True(outcome.IsNone);
        Assert.Null(testValueObject);
    }
}