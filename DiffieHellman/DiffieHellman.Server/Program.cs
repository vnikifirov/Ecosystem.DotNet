var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<DiffieHellman.Business.Services.Interfaces.IDiffieHellmanService, DiffieHellman.Business.Services.Implementation.DiffieHellmanService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

