using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AppBrix.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseApp(WebAppInitializerModule.CreateApp())
                .Build()
                .Run();
        }
    }
}
