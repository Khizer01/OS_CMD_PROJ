using System;
using OS_CMD_PROJECT.Commands;
using OS_CMD_PROJECT.Services;

namespace OS_CMD_PROJECT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize the command registry (stores all available commands)
            var registry = new CommandRegistry();

            // Initialize the AI client (used for interacting with Gemini API)
            var aiClient = new GeminiClient();

            // Register commands
            registry.Register(new HelpCommand(registry)); // Command to list all available commands
            registry.Register(new TaskCommands()); // Command to list running tasks (processes)
            registry.Register(new AskAiCommand(aiClient)); // Command to interact with the AI
            registry.Register(new CreateFileCommand()); // Command to create file
            registry.Register(new DeleteFileCommand()); // Command to delete file
            registry.Register(new DeleteDirectoryCommand()); // Command to delete directory
            registry.Register(new DisplayFileContents()); // Command to display file contents
            registry.Register(new Date()); // Command to display date


            registry.Register(new List()); //List current dir items
            registry.Register(new PresentWorkingDirectory()); // Command to display the current working directory
            registry.Register(new ChangeDirectory()); // Command to display the current working directory
            registry.Register(new CreateDirectory()); // Command to create directory
            registry.Register(new EditFile()); // Command to edit file content
            registry.Register(new OpenVs()); // Command to open vs-code

            registry.Register(new RenameCommand()); // Command to Rename file or Directory
            registry.Register(new ClearCommand()); // Command to Clear terminal screen
            registry.Register(new WhoAmICommand()); // Command to Display current username 
            registry.Register(new GetCpuUsageCommand());// Command to dispaly CPU usage 
            registry.Register(new KillProcessCommand());//Command to kill process using PID

            registry.Register(new FileCopyCommand());// Command to copy a single file (supports same or different directory)
            registry.Register(new DirectoryCopyCommand());// Command to copy a complete directory recursively
            registry.Register(new taskinfoCommand());// Command to display information about a file or directory
            registry.Register(new TimeCommand());// Command to display current system time
            registry.Register(new EchoCommand());// Command to print text to the console
            registry.Register(new TaskMonitorCommand()); // Command to monitor a file continuously

            // Display a welcome message
            Console.WriteLine("Welcome to OS CLI\nType 'help' to see available commands. Type 'exit' to quit.");

            while (true)
            {
                Console.Write(">> "); // Display the command prompt
                var input = Console.ReadLine(); // Read user input

                if (string.IsNullOrWhiteSpace(input))
                    continue; // Skip empty input

                input = input.Trim();

                // Exit the program if the user types 'exit'
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                // Split the input into command and arguments
                var parts = input.Split(' ');
                var commandName = parts[0]; // First word is the command name

                // Collect arguments manually for compatibility
                string[] commandArgs = new string[parts.Length - 1];
                for (int i = 1; i < parts.Length; i++)
                {
                    commandArgs[i - 1] = parts[i];
                }

                // Find the command in the registry
                var command = registry.Get(commandName);

                if (command == null)
                {
                    Console.WriteLine("Unknown command. Type 'help' to see available commands.");
                    continue;
                }

                // Execute the command
                try
                {
                    command.Execute(commandArgs).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Goodbye!");
        }
    }
}
