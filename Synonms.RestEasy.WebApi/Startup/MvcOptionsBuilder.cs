using Microsoft.AspNetCore.Mvc;

namespace Synonms.RestEasy.WebApi.Startup;

public class MvcOptionsBuilder
{
    public MvcOptions MvcOptions { get; }

    public MvcOptionsBuilder(MvcOptions mvcOptions)
    {
        MvcOptions = mvcOptions;
    }

    
}