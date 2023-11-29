var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getdata/{id}", (int id) =>
{
    var rndNumber = Random.Shared.Next(1, 101);
    if (rndNumber >= id)
    {
        return Results.StatusCode(500);
    }

    return Results.Ok();
});

app.Run();