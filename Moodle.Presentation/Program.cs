using Moodle.Application.Services;
using Moodle.Infrastructure.Repositories;

var userRepository = new TestUserRepository();
var authService = new AuthentificationService(userRepository);
        
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
