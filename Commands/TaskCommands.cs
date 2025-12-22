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


    public class TaskMonitorCommand : ICommand
    {
        public string Name => "task-monitor";
        public string Description => "Continuously monitor a file and show live info, including creation, modifications, size, history of changes, and memory usage";
        private int modificationCount = 0;
        private DateTime? firstChangeTime = null;
        private readonly List<DateTime> lastChanges = new List<DateTime>();
        private DateTime lastEventTime = DateTime.MinValue;
        private readonly int maxHistory = 5; // last 5 modifications

        public async Task Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: task-monitor <file name>");
                return;
            }

            string path = args[0];

            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist.");
                return;
            }

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                Filter = Path.GetFileName(path),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += (sender, e) =>
            {
                // Debounce rapid events
                if ((DateTime.Now - lastEventTime).TotalMilliseconds < 500) return;
                lastEventTime = DateTime.Now;

                modificationCount++;
                DateTime now = DateTime.Now;

                if (firstChangeTime == null)
                    firstChangeTime = now;

                lastChanges.Add(now);

                // Keep only last N changes
                if (lastChanges.Count > maxHistory)
                    lastChanges.RemoveAt(0);
            };

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Monitoring file. Press 'q' to stop.\n");

            try
            {
                while (true)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                        break;

                    try
                    {
                        FileInfo currentFile = new FileInfo(path);
                        Process currentProcess = Process.GetCurrentProcess();

                        Console.Clear();
                        Console.WriteLine("===== File Monitor Dashboard =====");
                        Console.WriteLine($"File: {currentFile.Name}");
                        Console.WriteLine($"Full Path: {currentFile.FullName}");
                        Console.WriteLine($"Created: {currentFile.CreationTime}");
                        Console.WriteLine($"Last Modified: {currentFile.LastWriteTime}");
                        Console.WriteLine($"Size: {currentFile.Length} bytes");
                        Console.WriteLine($"ReadOnly: {currentFile.IsReadOnly}, Hidden: {currentFile.Attributes.HasFlag(FileAttributes.Hidden)}");
                        Console.WriteLine($"Total Modifications Detected: {modificationCount}");
                        if (firstChangeTime != null)
                            Console.WriteLine($"First Change Detected At: {firstChangeTime}");
                        Console.WriteLine("Last Changes:");
                        if (lastChanges.Count == 0)
                            Console.WriteLine("  No changes yet");
                        else
                        {
                            foreach (var dt in lastChanges)
                                Console.WriteLine($"  {dt}");
                        }
                        Console.WriteLine($"Current Process RAM Usage: {currentProcess.WorkingSet64 / 1024.0 / 1024.0:F2} MB");
                        Console.WriteLine("Press 'q' to stop monitoring.");
                    }
                    catch (FileNotFoundException)
                    {
                        Console.Clear();
                        Console.WriteLine("File has been deleted. Monitoring stopped.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error accessing file: " + ex.Message);
                    }

                    await Task.Delay(1000); // Refresh every second
                }
            }
            finally
            {
                watcher.Dispose();
            }
        }
    }
}
