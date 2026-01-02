using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moodle.Application.Services;
using Moodle.Infrastructure.Persistence;
using Moodle.Infrastructure.Repositories;

var userRepository = new TestUserRepository();
var authService = new AuthentificationService(userRepository);

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=MoodleDb;Username=postgres;Password=postgres;"));

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

context.Database.Migrate();
DatabaseSeeder.Seed(context);
        
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

        var success = authService.RegisterUser(email, password);
        Console.WriteLine(success ? "Registered!" : "User already exists.");
    }
    else if (choice == "2")
    {
        Console.Write("Email: ");
        var email = Console.ReadLine()!;

        Console.Write("Password: ");
        var password = Console.ReadLine()!;

        var user = authService.LoginUser(email, password);
        Console.WriteLine(user != null ? $"Welcome {user.Email}" : "Invalid credentials");
    }
    else if (choice == "0")
    {
        break;
    }
}
