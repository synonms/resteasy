using Synonms.RestEasy.IoC;

namespace Synonms.RestEasy.Hypermedia.Ion;

public static class MvcOptionsBuilderExtensions
{
    public static MvcOptionsBuilder WithIon(this MvcOptionsBuilder mvcOptionsBuilder)
    {
        mvcOptionsBuilder.MvcOptions.InputFormatters.Add(new IonInputFormatter());
        mvcOptionsBuilder.MvcOptions.OutputFormatters.Add(new IonOutputFormatter());

        return mvcOptionsBuilder;
    }
}