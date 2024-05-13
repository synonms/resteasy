using Microsoft.AspNetCore.Mvc;

namespace Synonms.RestEasy.WebApi.Startup;

public static class MvcOptionsExtensions
{
    public static MvcOptions ConfigureForRestEasy(this MvcOptions mvcOptions)
    {
        mvcOptions.RespectBrowserAcceptHeader = true;

        return mvcOptions;
    }
    
    public static MvcOptions ClearFormatters(this MvcOptions mvcOptions)
    {
        mvcOptions.InputFormatters.Clear();
        mvcOptions.OutputFormatters.Clear();

        return mvcOptions;
    }
}