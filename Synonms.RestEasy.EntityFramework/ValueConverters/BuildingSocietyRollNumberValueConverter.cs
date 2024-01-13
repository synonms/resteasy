using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class BuildingSocietyRollNumberValueConverter : ValueConverter<BuildingSocietyRollNumber, string>
{
    public BuildingSocietyRollNumberValueConverter()
        : base(valueObject => valueObject.Value,
            value => BuildingSocietyRollNumber.Convert(value))
    {
    }
}