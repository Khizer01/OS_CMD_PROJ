using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace OS_CMD_PROJECT.Commands
{
    public class TaskCommands : ICommand
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

    public class MonitorProcessCommand : ICommand
    {
        public string Name => "monitor";
        public string Description => "Continuously monitor process CPU and RAM usage (press 'q' to stop)";

        public async Task Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: monitor <process-name | pid>");
                return;
            }

            await Task.Run(() =>
            {
                Process process = null;

                try
                {
                    int pid;
                    if (int.TryParse(args[0], out pid))
                    {
                        process = Process.GetProcessById(pid);
                    }
                    else
                    {
                        Process[] processes = Process.GetProcessesByName(args[0]);
                        if (processes.Length == 0)
                        {
                            Console.WriteLine("Process not found.");
                            return;
                        }
                        process = processes[0];
                    }

                    Console.WriteLine("========== PROCESS MONITOR ==========");
                    Console.WriteLine($"Process Name   : {process.ProcessName}");
                    Console.WriteLine($"Process ID     : {process.Id}");

                    long startRam = process.WorkingSet64;
                    TimeSpan startCpu = process.TotalProcessorTime;

                    Console.WriteLine($"RAM at Start   : {FormatMemory(startRam)}");
                    Console.WriteLine($"CPU at Start   : {startCpu.TotalMilliseconds:F0} ms");

                    Console.WriteLine("\nMonitoring CPU & RAM... (press 'q' to stop)\n");

                    bool stoppedByUser = false;

                    Thread keyListener = new Thread(() =>
                    {
                        while (true)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.Q)
                            {
                                stoppedByUser = true;
                                break;
                            }
                        }
                    });
                    keyListener.IsBackground = true;
                    keyListener.Start();

                    while (!process.HasExited && !stoppedByUser)
                    {
                        process.Refresh();
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] CPU: {process.TotalProcessorTime.TotalMilliseconds:F0} ms | RAM: {FormatMemory(process.WorkingSet64)}"
                        );
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine();

                    process.Refresh();

                    if (stoppedByUser)
                    {
                        Console.WriteLine("Monitoring stopped by user.");
                        Console.WriteLine($"CPU used so far : {process.TotalProcessorTime.TotalMilliseconds:F0} ms");
                        Console.WriteLine($"RAM at stop    : {FormatMemory(process.WorkingSet64)}");
                    }
                    else
                    {
                        Console.WriteLine("Process exited normally.");
                        Console.WriteLine($"Total CPU used : {process.TotalProcessorTime.TotalMilliseconds:F0} ms");
                        Console.WriteLine("RAM at exit    : 0.00 MB");
                    }

                    Console.WriteLine("=====================================");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error monitoring process: {ex.Message}");
                }
            });
        }

        private string FormatMemory(long bytes)
        {
            double kb = bytes / 1024.0;
            double mb = kb / 1024.0;
            return $"{mb:F2} MB";
        }
    }

}
