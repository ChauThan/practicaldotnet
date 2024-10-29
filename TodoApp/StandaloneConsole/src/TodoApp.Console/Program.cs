using Microsoft.Extensions.DependencyInjection;
using TodoApp.Console;
using TodoApp.Console.Data;

var services = new ServiceCollection();
services.AddScoped<DataContext>();

// create service provider
var serviceProvider = services.BuildServiceProvider();

// start App
var app = ActivatorUtilities.CreateInstance<App>(serviceProvider);
await app.RunAsync(args);
