using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EducationPhaseValueConverter : ValueConverter<EducationPhase, string>
{
    public EducationPhaseValueConverter()
        : base(valueObject => valueObject.Value,
            value => EducationPhase.Convert(value))
    {
    }
}