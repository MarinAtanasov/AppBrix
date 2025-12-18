using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Json;
using Microsoft.AspNetCore.Builder;

namespace AppBrix.WebApp;

internal static class Program
{
	public static void Main(string[] args)
	{
		WebApplication.CreateBuilder(args)
			.Build(App.Start<MainModule>(new ConfigService(new FilesConfigProvider(
				new JsonConfigSerializer(), "./Config", "json"))))
			.Run();
	}
}
