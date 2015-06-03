using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Logging;
using AppBrix.Logging.Entries;
using AppBrix.Logging.File.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace AppBrix.Console
{
    /// <summary>
    /// This is a sample console application using an AppBrix app.
    /// </summary>
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var app = App.Create();
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
                System.Console.WriteLine("Executed in: {0} seconds.", stopwatch.Elapsed.TotalSeconds);
            }
        }

        private static void Run(IApp app)
        {
            var generatorKey = typeof(MessageGenerator).FullName;
            app.GetFactory().Register<MessageGenerator>(() => new MessageGenerator("Test"));

            var cache = app.GetCache();
            cache[generatorKey] = app.GetFactory().Get<MessageGenerator>();

            for (var i = 0; i < 20; i++)
            {
                app.GetLog().Info(cache.Get<MessageGenerator>(generatorKey).Generate());
            }

            cache.Remove(generatorKey);
        }
    }
}
