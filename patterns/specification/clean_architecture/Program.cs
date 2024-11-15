using App.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure()
    .AddUseCases();

var app = builder.Build();

app.UseInfrastructure();

app.MapGet("/products", (GetProductsHandler handler) => handler.GetProducts());

app.Run();
