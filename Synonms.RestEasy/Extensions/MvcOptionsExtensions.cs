using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.IoC;

namespace Synonms.RestEasy.Extensions;

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