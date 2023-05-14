using Microsoft.EntityFrameworkCore;
using TODOList.BLL.Context.Context;
using TODOList.Business.Context;
using TODOList.Business.Repository.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<TodoListContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoListContext") ?? throw new InvalidOperationException("Connection string 'TodoListContext' not found."));
});

var dbContextBuilder = new DbContextOptionsBuilder();
//builder.Services.AddDbContextPool<TodoListContext>(opt => opt.SetOptions(builder.Configuration));
builder.Services.AddSingleton<Func<TodoListContext>>(s => () => new TodoListContext(dbContextBuilder.SetOptions(builder.Configuration).Options));

// Services and Repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
//services.AddScoped<ITaskService, TaskService>();

builder.WebHost.ConfigureKestrel();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
