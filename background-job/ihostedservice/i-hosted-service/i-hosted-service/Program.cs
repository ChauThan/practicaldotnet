using i_hosted_service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TimedHostedService>();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var host = builder.Build();
host.Run();
