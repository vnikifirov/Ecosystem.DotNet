using Microsoft.EntityFrameworkCore;
using TODOList.BLL.Context.Context;
using TODOList.BLL.Service.Implementation;
using TODOList.BLL.Service.Interface;
using TODOList.Business.Context;
using TODOList.Business.Repository.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<TodoListContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoListContext") ?? throw new InvalidOperationException("Connection string 'TodoListContext' not found."));
});

var dbContextBuilder = new DbContextOptionsBuilder();
//builder.Services.AddDbContextPool<TodoListContext>(opt => opt.SetOptions(builder.Configuration));
builder.Services.AddSingleton<Func<TodoListContext>>(s => () => new TodoListContext(dbContextBuilder.SetOptions(builder.Configuration).Options));

// Services and Repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskDummyService, TaskDummyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Middleware
app.UseCors(x => x.AllowAnyHeader()
.AllowAnyMethod()
.WithOrigins("http://localhost:3001"));
//app.UseAuthentication();
//app.UseAuthorization();


app.MapControllers();

app.Run();
