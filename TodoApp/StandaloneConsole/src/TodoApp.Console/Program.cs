using Microsoft.Extensions.DependencyInjection;
using TodoApp.Console;

var services = new ServiceCollection();

// create service provider
var serviceProvider = services.BuildServiceProvider();

// start App
var app = ActivatorUtilities.CreateInstance<App>(serviceProvider);
if (app is not null)
{
    await app.RunAsync(args);
}
