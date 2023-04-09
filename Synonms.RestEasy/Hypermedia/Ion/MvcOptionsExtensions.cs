using Microsoft.AspNetCore.Mvc;

namespace Synonms.RestEasy.Hypermedia.Ion;

public static class MvcOptionsExtensions
{
    public static MvcOptions WithIon(this MvcOptions mvcOptions)
    {
        mvcOptions.InputFormatters.Add(new IonInputFormatter());
        mvcOptions.OutputFormatters.Add(new IonOutputFormatter());

        return mvcOptions;
    }
}