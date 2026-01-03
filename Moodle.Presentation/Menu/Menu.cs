public class Menu
{
    private readonly List<MenuItem> _items = new();
    private readonly string _title;
    private readonly bool _isMainMenu;

    public Menu(string title, bool isMainMenu = false)
    {
        _title = title;
        _isMainMenu = isMainMenu;
    }

    public Menu AddItem(string label, Func<Task> action)
    {
        _items.Add(new MenuItem(label, action));
        return this;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_title);
            Console.WriteLine(new string('-', _title.Length));

            for (int i = 0; i < _items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_items[i].Label}");
            }
            
            Console.WriteLine(_isMainMenu ? "0. Izlaz" : "0. Povratak");
            Console.Write("\nOdabir: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                continue;
            }

            if (choice == 0) break;
            if (choice < 1 || choice > _items.Count)
            {
                Console.WriteLine("Nevažeći odabir");
                await Task.Delay(1000);
                continue;
            }

            Console.Clear();
            await _items[choice - 1].ExecuteAsync();
            
            Console.WriteLine("\nPritisnite bilo koju tipku za nastavak...");
            Console.ReadKey();
        }
    }

    private class MenuItem
    {
        public string Label { get; }
        private readonly Func<Task> _action;

        public MenuItem(string label, Func<Task> action)
        {
            Label = label;
            _action = action;
        }

        public Task ExecuteAsync() => _action();
    }
}