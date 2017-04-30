using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Yaml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AppBrix.WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Add AppBrix app.
            var configManager = new ConfigManager(new FilesConfigProvider("./Config", "yaml"), new YamlConfigSerializer());
            if (configManager.Get<AppConfig>().Modules.Count == 0)
                configManager.Get<AppConfig>().Modules.Add(ModuleConfigElement.Create<ConfigInitializerModule>());

            this.App = AppBrix.App.Create(configManager);
            this.App.Start();
        }

        public IConfigurationRoot Configuration { get; }

        public IApp App { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add AppBrix app to the DI container.
            services.AddApp(this.App);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add AppBrix logging provider.
            loggerFactory.AddProvider(this.App);

            app.UseMvc();

            // Dispose of AppBrix app during a graceful shutdown.
            lifetime.ApplicationStopped.Register(this.App.Stop);
        }
    }
}
