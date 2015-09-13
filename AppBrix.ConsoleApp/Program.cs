using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Json;
using System;
using System.Diagnostics;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// This is a sample console application using an AppBrix app.
    /// </summary>
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var app = App.Create(new ConfigManager(new FilesConfigProvider("./Config", "json"), new JsonConfigSerializer()));
            app.Start();
            try
            {
                Program.Run(app);
            }
            catch (Exception ex)
            {
                app.GetLog().Error("The application has stopped because of an error!", ex);
            }
            finally
            {
                app.Stop();
                Console.WriteLine("Executed in: {0} seconds.", stopwatch.Elapsed.TotalSeconds);
            }
        }

        private static void Run(IApp app)
        {
            app.GetFactory().Register<MessageGenerator>(() => new MessageGenerator("Test"));
            var generator = app.GetFactory().Get<MessageGenerator>();

            for (var i = 0; i < 20; i++)
            {
                app.GetLog().Info(generator.Generate());
            }
        }
    }
}
