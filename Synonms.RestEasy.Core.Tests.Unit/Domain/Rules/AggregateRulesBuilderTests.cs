using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules;
using Synonms.Functional;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain.Rules;

public class AggregateRulesBuilderTests
{
    private record TestIntValueObject : ValueObject<int>, IComparable, IComparable<TestIntValueObject>
    {
        private TestIntValueObject(int value) : base(value)
        {
        }
    
        public static implicit operator int(TestIntValueObject intValueObject) => intValueObject.Value;

        public static OneOf<Maybe<TestIntValueObject>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) => 
            value is null ? Maybe<TestIntValueObject>.None : new TestIntValueObject(value.Value);
    
        public int CompareTo(TestIntValueObject? other) => other is null ? 1 : Value.CompareTo(other);
    
        public int CompareTo(object? obj) => Value.CompareTo(obj);
    }

    private record TestStringValueObject : ValueObject<string>, IComparable, IComparable<TestStringValueObject>
    {
        private TestStringValueObject(string value) : base(value)
        {
        }
    
        public static implicit operator string(TestStringValueObject stringValueObject) => stringValueObject.Value;

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