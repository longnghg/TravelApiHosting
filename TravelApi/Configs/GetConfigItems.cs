using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace TravelApi.Configs
{
    public static class GetConfigItems
    {
        public static IWebHostEnvironment WebHostEnvironment { get; set; }
        public static IHttpContextAccessor HttpContextAccessor { get; set; }
        public static string GetUrlHost()
        {
            var scheme = HttpContextAccessor.HttpContext.Request.Scheme;
            var host = HttpContextAccessor.HttpContext.Request.Host;
            var url = $"{scheme}://{host}";

            return url.ToString().Substring(0, url.ToString().Length - 1);
        }
    }
}
