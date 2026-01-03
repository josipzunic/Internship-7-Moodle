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

services.AddScoped<AuthentificationService>();

var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

var authService = serviceProvider.GetRequiredService<AuthentificationService>();
        
while (true)
{
    Console.WriteLine("1. Register");
    Console.WriteLine("2. Login");
    Console.WriteLine("0. Exit");
    Console.Write("Choice: ");

    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Email: ");
        var email = Console.ReadLine()!;

        Console.Write("Password: ");
        var password = Console.ReadLine()!;

        var success = await authService.RegisterUserAsync(email, password);
        Console.WriteLine($"{success.Value}, {success.ValidationMessage}");
    }
    else if (choice == "2")
    {
        Console.Write("Email: ");
        var email = Console.ReadLine()!;

        Console.Write("Password: ");
        var password = Console.ReadLine()!;

        var user = await authService.LoginUserAsync(email, password);
        if  (user.Value != null) Console.WriteLine($"{user.ValidationMessage}");
    }
    else if (choice == "0")
    {
        break;
    }
}
