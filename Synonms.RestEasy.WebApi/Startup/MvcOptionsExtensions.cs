using Microsoft.AspNetCore.Mvc;

namespace Synonms.RestEasy.WebApi.Startup;

public static class MvcOptionsExtensions
{
    public static MvcOptionsBuilder ConfigureForRestEasy(this MvcOptions mvcOptions)
    {
        mvcOptions.InputFormatters.Clear();
        mvcOptions.OutputFormatters.Clear();
        
        mvcOptions.RespectBrowserAcceptHeader = true;

        return new MvcOptionsBuilder(mvcOptions);
    }
}