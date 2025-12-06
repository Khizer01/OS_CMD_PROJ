using System.Collections.Generic;
using System.Linq;

namespace OS_CMD_PROJECT.Commands
{
    // Registry to store and manage all available commands
    public class CommandRegistry
    {
        private readonly List<ICommand> _commands = new List<ICommand>(); // List of registered commands

        // Add a new command to the registry
        public void Register(ICommand command)
        {
            // Avoid duplicate commands
            if (_commands.Any(c => c.Name == command.Name))
                return;

            _commands.Add(command);
        }

        // Get a command by its name
        public ICommand Get(string name)
        {
            return _commands.FirstOrDefault(c => c.Name == name);
        }

        // Get all registered commands
        public IEnumerable<ICommand> GetAll() => _commands;
    }
}
