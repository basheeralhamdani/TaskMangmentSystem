using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.API.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, TaskManagementSystem.API.Services.CustomUserIdProvider>();


// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<TaskManagementSystem.Infrastructure.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories and unit of work
builder.Services.AddScoped<TaskManagementSystem.Infrastructure.Data.IUnitOfWork, TaskManagementSystem.Infrastructure.Data.UnitOfWork>();
builder.Services.AddScoped<TaskManagementSystem.Core.Interfaces.ITaskRepository, TaskManagementSystem.Infrastructure.Repositories.TaskRepository>();
builder.Services.AddScoped<TaskManagementSystem.Core.Interfaces.IUserRepository, TaskManagementSystem.Infrastructure.Repositories.UserRepository>();

// Add services
builder.Services.AddScoped<TaskManagementSystem.Core.Interfaces.ITaskService, TaskManagementSystem.Core.Services.TaskService>();
builder.Services.AddScoped<TaskManagementSystem.Core.Interfaces.IUserService, TaskManagementSystem.Core.Services.UserService>();
builder.Services.AddScoped<TaskManagementSystem.Core.Interfaces.INotificationService, TaskManagementSystem.API.Services.SignalRNotificationService>();

// Add authentication and authorization
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "TaskManagementSystem.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("SystemAdministrator"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager", "SystemAdministrator"));
    options.AddPolicy("RequireTaskAdministratorRole", policy => policy.RequireRole("TaskAdministrator", "SystemAdministrator"));
});

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Dev helper: ensure database exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskManagementSystem.Infrastructure.Data.ApplicationDbContext>();
    db.Database.EnsureCreated();
}

// Seed admin user (works in all environments)
await StartupSeeder.SeedAdminAsync(app.Services);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger in Development
    app.UseSwagger();
    app.UseSwaggerUI();
}


else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<TaskManagementSystem.API.Hubs.NotificationsHub>("/hubs/notifications");

app.Run();
