using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace OS_CMD_PROJECT.Commands
{
    public class ListTasksCommand : ICommand
    {
        public string Name => "listTasks";
        public string Description => "Terminal task manager (Press Q or ESC to exit)";

        public Task Execute(string[] args)
        {
            Console.Clear();
            Console.CursorVisible = false;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
                        break;
                }

                Console.SetCursorPosition(0, 0);

                Console.WriteLine(" ID     Process Name               Memory (MB)");
                Console.WriteLine("-----------------------------------------------");

                var processes = Process.GetProcesses();

                foreach (var process in processes)
                {
                    try
                    {
                        string name = process.ProcessName;

                        if (name.Length > 25)
                            name = name.Substring(0, 22) + "...";

                        double memoryMb = process.WorkingSet64 / (1024.0 * 1024.0);

                        Console.WriteLine(
                            $"{process.Id,-6} {name,-25} {memoryMb,10:F2}"
                        );
                    }
                    catch
                    {
                        // Ignore inaccessible system processes
                    }
                }

                Console.WriteLine("\nPress Q or ESC to exit...");
                Thread.Sleep(3000);
            }

            Console.CursorVisible = true;
            Console.Clear();
            return Task.CompletedTask;
        }
    }

    public class KillProcessCommand : ICommand
    {
        public string Name => "kill";
        public string Description => "Kills a process using PID";

        public Task Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: killl <Pid>");
                return Task.CompletedTask;
            }

            if (int.TryParse(args[0], out int pid))
            {
                try
                {
                    Process.GetProcessById(pid).Kill();
                    Console.WriteLine($"Process {pid} killed successfully.");
                }
                catch
                {
                    Console.WriteLine($"Failed to kill process {pid}.");
                }
            }
            else
            {
                Console.WriteLine("Invalid PID.");
            }

                Console.SetCursorPosition(0, 0);

                Console.WriteLine(" ID     Process Name               Memory (MB)");
                Console.WriteLine("-----------------------------------------------");

                var processes = Process.GetProcesses();

                foreach (var process in processes)
                {
                    try
                    {
                        string name = process.ProcessName;

                        if (name.Length > 25)
                            name = name.Substring(0, 22) + "...";

                        double memoryMb = process.WorkingSet64 / (1024.0 * 1024.0);

                        Console.WriteLine(
                            $"{process.Id,-6} {name,-25} {memoryMb,10:F2}"
                        );
                    }
                    catch
                    {
                        // Ignore inaccessible system processes
                    }
                }

                Console.WriteLine("\nPress Q or ESC to exit...");
                Thread.Sleep(3000);
            }

            Console.CursorVisible = true;
            Console.Clear();
            return Task.CompletedTask;
        }
    }
    

   
    public class GetCpuUsageCommand : ICommand
    {
        public string Name => "cpu-usage";
        public string Description => "Displays CPU usage";

        public async Task Execute(string[] args)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        Console.WriteLine($"\nCPU Usage: {obj["LoadPercentage"]}%");
                    }
                }

            }
            catch
            {
                Console.WriteLine("Unable to retrieve CPU usage.");
            }

            await Task.Delay(2000);
        }
    }
}
