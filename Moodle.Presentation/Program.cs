using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moodle.Application.Interfaces;
using Moodle.Application.Services;
using Moodle.Infrastructure.Persistence;
using Moodle.Infrastructure.Repositories;

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=MoodleDb;Username=postgres;Password=postgres;"
    ));

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ICourseRepository, CourseRepository>();

services.AddScoped<AuthentificationService>();


var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

var authService = serviceProvider.GetRequiredService<AuthentificationService>();
var courseRepository = serviceProvider.GetRequiredService<ICourseRepository>();
        
var menuActions = new MenuActions(authService, courseRepository);

var mainMenu = new Menu("Moodle Authentication ")
    .AddItem("Register", menuActions.RegisterAsync)
    .AddItem("Login", menuActions.LoginAsync);

await mainMenu.RunAsync();

