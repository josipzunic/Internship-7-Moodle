using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Domain.Enums;

public class MenuActions
{
    private readonly AuthentificationService _authService;
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IPrivateChatRepository _privateChatRepository;
    private readonly IAdminRepository _adminRepository;
    private User? _currentUser;

    public MenuActions(AuthentificationService authService, IUserCourseRepository userCourseRepository,  
        IEnrollmentRepository enrollmentRepository,  INotificationRepository notificationRepository,
        IMaterialRepository materialRepository,  IPrivateChatRepository privateChatRepository, IAdminRepository adminRepository)
    {
        _authService = authService;
        _userCourseRepository = userCourseRepository;
        _enrollmentRepository = enrollmentRepository;
        _notificationRepository = notificationRepository;
        _materialRepository = materialRepository;
        _privateChatRepository = privateChatRepository;
        _adminRepository = adminRepository;
    }

    public async Task RegisterAsync()
    {
        Console.Write("Email: ");
        var email = Console.ReadLine()!;

        Console.Write("Lozinka: ");
        var password = Console.ReadLine()!;
        
        Console.Write("Ponovite lozinku: ");
        var repeeatPassword = Console.ReadLine()!;

        var generateCaptcha = _authService.GenerateCaptcha();
        Console.WriteLine("Captcha: " + generateCaptcha);
        Console.Write("Ponovite: ");
        var enterCaptcha = Console.ReadLine()!;

        var result = await _authService.RegisterUserAsync(email, password, repeeatPassword, enterCaptcha, generateCaptcha);
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
            Console.Write("Anti bot timeout 30 sekundi: ");
            for (int i = 30; i > 0; i--)
            {
                Console.WriteLine($"Timeout: {i}");
                await Task.Delay(1000);
            }
        }
    }

    private async Task ShowDashboardAsync()
    {
        if (_currentUser == null) return;

        var dashboard = new Menu($"==={_currentUser.Email} ===", false, LogoutAsync);
        
        dashboard.AddItem("Privatni chat", PrivateChatAsync);
        
        switch (_currentUser.Role)
        {
            case Role.Student:
                dashboard.AddItem("Moji kolegiji", MyCoursesStudentAsync);
                break;
            
            case Role.Professor:
                dashboard.AddItem("Moji kolegiji", MyCoursesProfessorAsync);
                dashboard.AddItem("Upravljanje kolegijima", ManageCoursesAsync);
                break;
            
            case Role.Admin:
                dashboard.AddItem("Upravljanje korisnicima", ManageUsersAsync);
                break;
        }

        await dashboard.RunAsync();
    }
    private async Task LogoutAsync()
    {
        _currentUser = null;
        Console.WriteLine("\nUspješno ste se odjavili.");
        await Task.Delay(1000);
        
    }
    private async Task MyCoursesStudentAsync()
    {
        var coursesMenu = new Menu("=== Moji kolegiji ===");
        var courses = await _userCourseRepository.GetCoursesAsync(_currentUser.Id);
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
    
    private async Task MyCoursesProfessorAsync()
    {
        var coursesMenu = new Menu("=== Moji kolegiji ===");
        var courses = await _userCourseRepository.GetCoursesOverviewForProfessorAsync(_currentUser.Id);
        foreach (var course in courses)
            coursesMenu.AddItem($"{course.CourseName}", async () =>
            {
                var subCoursesMenu = new Menu("=== Moji kolegiji ===");
                subCoursesMenu.AddItem("Pregled studenata", async () =>
                {
                    if (course.Students.Count == 0) Console.WriteLine("Nema upisanih studenata");
                    else
                        foreach (var student in course.Students)
                            Console.WriteLine($"{student.Email}");
                    await Task.CompletedTask;
                });
                subCoursesMenu.AddItem("Obavijesti", async () =>
                {
                    if (course.Notifications.Count == 0) Console.WriteLine("Nema obavijesti");
                    else
                        foreach (var notification in course.Notifications)
                            Console.WriteLine(
                                $"{notification.CreatedAt} - {notification.Title}: {notification.Content}");
                    await Task.CompletedTask;

                });
                subCoursesMenu.AddItem("Materijali", async () =>
                {
                    if (course.Materials.Count == 0) Console.WriteLine("Nema materijala");
                    else
                        foreach (var material in course.Materials)
                            Console.WriteLine(
                                $"{material.CreatedAt} - {material.Name} - {material.Url}");
                    await Task.CompletedTask;
                });
                
                await subCoursesMenu.RunAsync();
                await Task.CompletedTask;
            });

        await coursesMenu.RunAsync();
    }

    private async Task ManageCoursesAsync()
    {
        var manageMenu = new Menu("=== Upravljanje kolegijima ===");
        var courses = await _userCourseRepository.GetCoursesOverviewForProfessorAsync(_currentUser.Id);
        foreach (var course in courses)
            manageMenu.AddItem($"{course.CourseName}", async () =>
            {
                var subManageMenu = new Menu("=== Upravljanje kolegijima ===");
                subManageMenu.AddItem("Dodavanje studenta", async () =>
                {
                    var addStudentToCourseMenu = new Menu("=== Dodavanje studenata ===");
                    var students = await _userCourseRepository.GetStudentsNotEnrolledInCourseAsync(course.CourseId);
                    Console.WriteLine(students.Count);
                    foreach (var student in students)
                        addStudentToCourseMenu.AddItem($"{student.Email}", async () =>
                        {
                            await _enrollmentRepository.EnrollStudentAsync(student.Id, course.CourseId);
                            Console.WriteLine("Student uspješno dodan");
                            students.Remove(student);
                            await Task.CompletedTask;
                        });
                    await addStudentToCourseMenu.RunAsync();
                    await Task.CompletedTask;
                });
                subManageMenu.AddItem("Dodavanje obavijesti", async () =>
                {
                    Console.Write("Naslov: ");
                    var title = Console.ReadLine();
                    Console.WriteLine("Sadržaj: ");
                    var content = Console.ReadLine();
                    
                    await _notificationRepository.AddNotificationAsync(title, content, course.CourseId, _currentUser.Id);
                    await Task.CompletedTask;

                });
                subManageMenu.AddItem("Dodavanje materijala", async () =>
                {
                    Console.Write("Ime: ");
                    var name = Console.ReadLine();
                    Console.WriteLine("Url: ");
                    var url = Console.ReadLine();
                    
                    await _materialRepository.AddMaterialAsync(name, url, course.CourseId, _currentUser.Id);
                    await Task.CompletedTask;
                });
                
                await subManageMenu.RunAsync();
                await Task.CompletedTask;
            });

        await manageMenu.RunAsync();
    }

    private async Task PrivateChatAsync()
    {
        var chatMenu = new Menu("=== Poruke ===");
        chatMenu.AddItem("Nova poruka", async () =>
        {
            var subChatMenu = new Menu("=== Novi razgovor ===");
            var users = await _privateChatRepository.GetUsersWithoutConversationAsync(_currentUser.Id);
            foreach (var user in users)
                subChatMenu.AddItem($"{user.Email} - {user.Role}", async () =>
                {
                    await _privateChatRepository.OpenPrivateChatAsync(_currentUser.Id,  user.Id);
                    await Task.CompletedTask;
                });
            
            await subChatMenu.RunAsync();
        });
        chatMenu.AddItem("Moji razgovori", async () =>
        {
            var subChatMenu = new Menu("=== Postojeći razgovori ===");
            var users = await _privateChatRepository.GetUsersWithConversationAsync(_currentUser.Id);
            foreach (var user in users)
                subChatMenu.AddItem($"{user.Email} - {user.Role}", async () =>
                {
                    await _privateChatRepository.OpenPrivateChatAsync(_currentUser.Id,  user.Id);
                    await Task.CompletedTask;
                });
            
            await subChatMenu.RunAsync();
        });
        
        await chatMenu.RunAsync();
    }

    private async Task ManageUsersAsync()
    {
        var adminMenu = new  Menu("=== Upravljanje korisnicima ===");
        var users = await _adminRepository.GetAllUsers(_currentUser.Id);

        adminMenu.AddItem("Brisanje korisnika", async () =>
        {
            var subAdminMenu = new Menu("Brisanje korisnika");
            foreach (var user in users)
                subAdminMenu.AddItem($"{user.Email} -  {user.Role}", async () =>
                {
                    await _adminRepository.DeleteUserAsync(user.Id);
                    Console.WriteLine($"korisnik {user.Id} - {user.Email} -  {user.Role} uspješno izbrisan iz baze");
                    await Task.CompletedTask;
                });
            await subAdminMenu.RunAsync();
        });

        adminMenu.AddItem("Uređivanje emaila", async () =>
        {
            var subAdminMenu = new Menu("Uređivanje emaila");
            foreach (var user in users)
                subAdminMenu.AddItem($"{user.Email} -  {user.Role}", async () =>
                {
                    Console.Write("Unesite novu email adresu: ");
                    var newEmail = Console.ReadLine()!;
                    var success = await _adminRepository.UpdateUserEmailAsync(user.Id,  newEmail);
                    await Task.CompletedTask;
                    if(success)
                        Console.WriteLine($"korisnik {user.Id} - {user.Email} -  {user.Role} uspješno izmijenjen u bazi podataka");
                    else
                        Console.WriteLine("Korisnik nije izmijenjen");
                });
            await subAdminMenu.RunAsync();
        });

        adminMenu.AddItem("Promjena titule", async () =>
        {
            var subAdminMenu = new Menu("Promjena titule");
            foreach (var user in users)
                subAdminMenu.AddItem($"{user.Email} -  {user.Role}", async () =>
                {
                    await _adminRepository.UpdateUserRoleAsync(user.Id, user.Role);
                    Console.WriteLine($"korisniku {user.Email} promijenjena je titula u bazi podataka");
                    await Task.CompletedTask;
                });
            await subAdminMenu.RunAsync();
        });
        
        await adminMenu.RunAsync();
    }
}