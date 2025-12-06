using System.Threading.Tasks;

namespace OS_CMD_PROJECT.Commands
{
    // Interface for all commands
    // Every command must implement this interface
    public interface ICommand
    {
        // The name of the command (e.g., "help")
        string Name { get; }

        // A short description of what the command does
        string Description { get; }

        // The method that gets executed when the command is called
        // Example: If the user types "help", this method will run
        Task Execute(string[] args);
    }
}
