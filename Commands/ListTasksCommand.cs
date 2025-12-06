using System;
using System.Threading.Tasks;

namespace OS_CMD_PROJECT.Commands
{
    // Command to list all currently running tasks (processes) on the operating system
    // This command will display details such as Process ID (PID) and Process Name
    public class ListTasksCommand : ICommand
    {
        public string Name => "listTasks"; // Command name
        public string Description => "List all running tasks (processes) with their details."; // Command description

        public Task Execute(string[] args)
        {
            // Placeholder implementation
            // Future implementation: Use System.Diagnostics.Process to get all running processes
            // Example:
            // foreach (var process in Process.GetProcesses())
            // {
            //     Console.WriteLine($"PID: {process.Id}, Name: {process.ProcessName}");
            // }

            Console.WriteLine("This command will list all running tasks (processes) in the future.");
            Console.WriteLine("Details such as Process ID (PID) and Process Name will be displayed.");

            return Task.CompletedTask;
        }
    }
}
