using System;
using System.Linq;
using System.Threading.Tasks;

namespace OS_CMD_PROJECT.Commands
{
    // Command to display all available commands
    // This command will list all the commands registered in the application
    public class HelpCommand : ICommand
    {
        private readonly CommandRegistry _registry; // Reference to the command registry

        public HelpCommand(CommandRegistry registry)
        {
            _registry = registry;
        }

        public string Name => "help"; // Command name
        public string Description => "Show a list of available commands."; // Command description

        public Task Execute(string[] args)
        {
            Console.WriteLine("Available commands:");

            // List all registered commands
            foreach (var cmd in _registry.GetAll().OrderBy(c => c.Name))
            {
                Console.WriteLine($"  {cmd.Name} - {cmd.Description}");
            }

            Console.WriteLine("  exit - Exit the CLI"); // Add exit command explicitly
            return Task.CompletedTask;
        }
    }
}
