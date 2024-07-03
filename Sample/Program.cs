// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoAttributedConfiguration;
using Newtonsoft.Json;
using Sample;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

Console.WriteLine($"Using {environment} Environment");

var configurationRoot = new ConfigurationBuilder()
	.AddJsonFile("settings.json", optional: false, reloadOnChange: false)
	.AddJsonFile($"settings.{environment}.json", optional: true, reloadOnChange: false)
	.AddEnvironmentVariables()
	.Build();

var serviceProvider = new ServiceCollection()
	.AddAttributedConfigurations(configurationRoot)
	.BuildServiceProvider();

var rootConfiguration = serviceProvider.GetRequiredService<RootConfiguration>();
var rootConfigurationJson = JsonConvert.SerializeObject(rootConfiguration, Formatting.Indented);
Console.WriteLine($"{nameof(RootConfiguration)} :");
Console.WriteLine(rootConfigurationJson);
Console.WriteLine();

var nestedConfiguration = serviceProvider.GetRequiredService<NestedConfiguration>();
var nestedConfigurationJson = JsonConvert.SerializeObject(nestedConfiguration, Formatting.Indented);
Console.WriteLine($"{nameof(NestedConfiguration)} :");
Console.WriteLine(nestedConfigurationJson);
