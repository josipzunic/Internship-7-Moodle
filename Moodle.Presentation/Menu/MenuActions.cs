using Moodle.Application.Interfaces;
using Moodle.Application.Services;
using Moodle.Domain.Entities;
using Moodle.Domain.Enums;

public class MenuActions
{
    private readonly AuthentificationService _authService;
    private readonly ICourseRepository _courseRepository;
    private User? _currentUser;

    public MenuActions(AuthentificationService authService, ICourseRepository courseRepository)
    {
        _authService = authService;
        _courseRepository = courseRepository;
    }

    public async Task RegisterAsync()
    {
        Console.Write("Email: ");
        var email = Console.ReadLine()!;

        Console.Write("Lozinka: ");
        var password = Console.ReadLine()!;

        var result = await _authService.RegisterUserAsync(email, password);
        Console.WriteLine($"\n{result.ValidationMessage}");
    }

    public async Task LoginAsync()
    {
        Console.Write("Email: ");
        var email = Console.ReadLine()!;

        Console.Write("Lozinka: ");
        var password = Console.ReadLine()!;

        var result = await _authService.LoginUserAsync(email, password);
        
        if (result.Value != null)
        {
            _currentUser = result.Value;
            Console.WriteLine($"\nDobrodošli {result.Value.Email}");
            await Task.Delay(1500);
            await ShowDashboardAsync();
        }
        else
        {
            Console.WriteLine($"\n{result.ValidationMessage}");
        }
    }

    private async Task ShowDashboardAsync()
    {
        if (_currentUser == null) return;

        var dashboard = new Menu($"==={_currentUser.Email} ===");
        
        dashboard.AddItem("Privatni chat", PrivateChatAsync);
        dashboard.AddItem("Odjava", LogoutAsync);
        
        switch (_currentUser.Role)
        {
            case Role.Student:
                dashboard.AddItem("Moji kolegiji", MyCoursesStudentAsync);
                break;
            
            /*case Role.Professor:
                dashboard.AddItem("Moji kolegiji", MyCoursesProfessorAsync);
                dashboard.AddItem("Upravljanje kolegijima", ManageCoursesAsync);
                break;
            
            case Role.Admin:
                dashboard.AddItem("Upravljanje korisnicima", ManageUsersAsync);
                break;*/
        }

        await dashboard.RunAsync();
    }
    private async Task PrivateChatAsync()
    {
        var chatMenu = new Menu("=== Privatni chat ===");
        chatMenu.AddItem("Pregled poruka", async () => 
        {
            Console.WriteLine("Lista privatnih poruka...");
            await Task.CompletedTask;
        });
        chatMenu.AddItem("Nova poruka", async () => 
        {
            Console.WriteLine("Slanje nove poruke...");
            await Task.CompletedTask;
        });

        await chatMenu.RunAsync();
    }

    private async Task LogoutAsync()
    {
        _currentUser = null;
        Console.WriteLine("\nUspješno ste se odjavili.");
        await Task.Delay(1000);
        
    }
    private async Task MyCoursesStudentAsync()
    {
        var coursesMenu = new Menu("=== Moji kolegiji (Student) ===");
        var courses = await _courseRepository.GetCoursesAsync(_currentUser.Id);
        foreach (var course in courses)
            coursesMenu.AddItem($"{course.CourseName}", async () =>
            {
                Console.WriteLine("===Obavijesti===");
                if(course.Notifications.Count == 0) Console.WriteLine("Nema obavijesti");
                else
                    foreach (var notification in course.Notifications)
                        Console.WriteLine($"{notification.CreatedAt} - {notification.ProfessorEmail} - {notification.Title}: {notification.Content}");
                Console.WriteLine("===Materijali===");
                foreach (var material in course.Materials)
                    Console.WriteLine($"{material.CreatedAt} - {material.Name} - {material.Url}");
                await Task.CompletedTask;
            });

        await coursesMenu.RunAsync();
    }
    
    
}