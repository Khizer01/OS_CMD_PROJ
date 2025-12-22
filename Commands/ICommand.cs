
﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

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

    public class CreateFileCommand : ICommand
    {
        public string Name => "create-file";

        public string Description => "Creates a new file";

        public async Task Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: create-file <file_name> ");
                return;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();

                using (var fs = File.Create(filePath))
                {
                    await fs.FlushAsync(); // ensure async creation
                }

                Console.WriteLine($"File Created: {filePath}");
            }
            else
            {
                Console.WriteLine($"File Already Exists: {filePath}");
            }

        }
    }

    public class DeleteFileCommand : ICommand
    {
        public string Name => "delete-file";
        public string Description => "Deletes a File";

        public async Task Execute(String[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: delete-file <file-name>");
                return;

            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} doesn't exsit");
                return;
            }
            else
            {
                File.Delete(filePath);
                await Task.Run(() => File.Delete(filePath));
                Console.WriteLine($"Deleted File: {filePath}");
            }

        }
    }

    public class DeleteDirectoryCommand : ICommand
    {
        public string Name => "delete-dir";
        public string Description => "Deletes a Directory";

        public async Task Execute(String[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: delete-dir <file-name>");
                return;

            }

            string dirPath = args[0];

            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine($"{dirPath} does not exist");
                return;
            }
            else
            {
                Directory.Delete(dirPath, false);
                await Task.Run(() => File.Delete(dirPath));
                Console.WriteLine($"Deleted Directory: {dirPath}");
            }

        }
    }

    public class DisplayFileContents : ICommand
    {
        public string Name => "cat";

        public string Description => "Dispalys File Content! ";

        public async Task Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: cat <file-name>");
                return;
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File doesn't Exsist!");
                return;
            }
            else
            {
                string fileContent = await Task.Run(() => File.ReadAllText(filePath));
                Console.WriteLine("----------- File Content ---------------");
                Console.WriteLine(fileContent);
                Console.WriteLine("----------- END ---------------");

            }
        }
    }

    public class Date : ICommand
    {
        public string Name => "date";

        public string Description => "Displays Curent date and time! ";

        public async Task Execute(string[] args)
        {
            await Task.Run(() =>
            {
                string date = DateTime.Now.ToString("F");
                Console.WriteLine(date);
            });
        }
    }

    public class CreateDirectory : ICommand {
        public string Name => "make-dir";

        public string Description => "Create a new directory ";

        public async Task Execute(string[] args) {
            await Task.Run(() => {
                if (args.Length == 0) {
                    Console.WriteLine("Usage: make-dir <directory-name>");
                    return;
                }

                string dirName = args[0];
                if(Directory.Exists(dirName)) {
                    Console.WriteLine($"{dirName} already exists");
                    return;
                }

                Directory.CreateDirectory(dirName);
                Console.WriteLine($"Directory Created: {dirName} sucessfully");
            });
        }
    }

    public class List : ICommand {
        public string Name => "ls";

        public string Description => "Displays list of elements in the current directory ";

        public async Task Execute(string[] args) {
            await Task.Run(() => {
                string directory = Directory.GetCurrentDirectory();
                string[] dirItems = Directory.GetFileSystemEntries(directory);
                foreach(var item in dirItems) {
                    Console.WriteLine(item);
                }
            });
        }
    }

    public class PresentWorkingDirectory : ICommand {
        public string Name => "pwd";

        public string Description => "Displays current directory";

        public async Task Execute(string[] args) {
            await Task.Run(() => {
                string directory = Directory.GetCurrentDirectory();
                Console.WriteLine(directory);
            });
        }
    }
    public class ChangeDirectory : ICommand {
        public string Name => "cd";

        public string Description => "Change Directory";

        public async Task Execute(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Usage: cd <directory>");
                return;
            }

            string targetPath = args[0];

            try {
                if (!Directory.Exists(targetPath)) {
                    Console.WriteLine("Directory does not exist.");
                    return;
                }

                await Task.Run(() => {
                    Directory.SetCurrentDirectory(targetPath);
                    Console.WriteLine("Current Directory:");
                    Console.WriteLine(Directory.GetCurrentDirectory());
                });
            }
            catch (Exception ex) {
                Console.WriteLine("Error changing directory: " + ex.Message);
            }
        }
    }

    public class EditFile : ICommand {
        public string Name => "edit-file";

        public string Description => "Edit file content";

        public async Task Execute(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Usage: edit-file <filename>");
                return;
            }

            string targetPath = args[0];
            if (!File.Exists(targetPath)) {
                Console.WriteLine("File does not exist!");
                return;
            }

            await Task.Run(() => {
                Process.Start("notepad.exe", targetPath);
            });
        }
    }

    public class RenameCommand : ICommand
    {
        public string Name => "rename";

        public string Description => "Renames a file or directory. Usage: rename <old_name> <new_name>";

        public async Task Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: rename <old_name> <new_name>");
                return;
            }

            string oldPath = args[0];
            string newPath = args[1];

            if (File.Exists(oldPath))
            {
                await Task.Run(() => File.Move(oldPath, newPath));
                Console.WriteLine($"File renamed: {oldPath} --> {newPath}");
            }
            else if (Directory.Exists(oldPath))
            {
                await Task.Run(() => Directory.Move(oldPath, newPath));
                Console.WriteLine($"Directory renamed: {oldPath} --> {newPath}");
            }
            else
            {
                Console.WriteLine("File or directory does not exist.");
            }
        }
    }

    public class ClearCommand : ICommand
    {
        public string Name => "clear";

        public string Description => "Clears the terminal screen.";

        public Task Execute(string[] args)
        {
            Console.Clear();
            return Task.CompletedTask;
        }
    }

    public class WhoAmICommand : ICommand
    {
        public string Name => "whoami";
        public string Description => "Displays the current user name.";

        public Task Execute(string[] args)
        {
            Console.WriteLine(Environment.UserName);
            return Task.CompletedTask;
        }
    }

    public class OpenVs : ICommand
    {
        public string Name => "code.";

        public string Description => "Open Visual Studio Code";

        public async Task Execute(string[] args)
        {
            await Task.Run(() => {
                Process.Start("code");
            });
        }
    }
    public class FileCopyCommand : ICommand
    {
        public string Name => "copyfile";
        public string Description => "Copy a single file (same or different directory)";

        public async Task Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: copyfile <source-file> <destination-file-or-directory>");
                return;
            }

            string src = args[0];
            string dst = args[1];

            await Task.Run(() =>
            {
                if (!File.Exists(src))
                {
                    Console.WriteLine("Source file does not exist.");
                    return;
                }

                // If destination is a directory, keep the same filename
                if (Directory.Exists(dst))
                {
                    dst = Path.Combine(dst, Path.GetFileName(src));
                }
                else
                {
                    // Ensure destination directory exists
                    string dstDir = Path.GetDirectoryName(dst);
                    if (!Directory.Exists(dstDir))
                    {
                        Directory.CreateDirectory(dstDir);
                    }
                }

                // Prevent copying onto itself
                if (Path.GetFullPath(src).Equals(Path.GetFullPath(dst), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Source and destination are the same file. Copy skipped.");
                    return;
                }

                File.Copy(src, dst, true);
                Console.WriteLine($"File copied successfully from {src} to {dst}");
            });
        }
    }
    public class DirectoryCopyCommand : ICommand
    {
        public string Name => "copydir";
        public string Description => "Copy a directory recursively";

        public async Task Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: copydir <source-directory> <destination-directory>");
                return;
            }

            string srcDir = args[0];
            string dstDir = args[1];

            await Task.Run(() =>
            {
                if (!Directory.Exists(srcDir))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return;
                }

                CopyDirectoryRecursive(srcDir, dstDir);
                Console.WriteLine($"Directory copied successfully from {srcDir} to {dstDir}");
            });
        }

        private void CopyDirectoryRecursive(string srcDir, string dstDir)
        {
            // Append source folder name to destination if it doesn't exist
            string dstFolder = Path.Combine(dstDir, Path.GetFileName(srcDir.TrimEnd(Path.DirectorySeparatorChar)));
            if (!Directory.Exists(dstFolder))
            {
                Directory.CreateDirectory(dstFolder);
            }

            // Copy all files
            foreach (var file in Directory.GetFiles(srcDir))
            {
                string dstFile = Path.Combine(dstFolder, Path.GetFileName(file));
                File.Copy(file, dstFile, true);
            }

            // Copy all subdirectories recursively
            foreach (var dir in Directory.GetDirectories(srcDir))
            {
                CopyDirectoryRecursive(dir, dstFolder);
            }
        }
    }



    public class taskinfoCommand : ICommand
    {
        public string Name => "TASK-INFORMATION";
        public string Description => "Show information about file or directory";

        public async Task Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: task-info <file or directory name>");
                return;
            }

            string path = args[0];

            await Task.Run(() =>
            {
                if (File.Exists(path))
                {
                    FileInfo f = new FileInfo(path);
                    Console.WriteLine($"File: {f.Name}");
                    Console.WriteLine($"Size: {f.Length} bytes");
                    Console.WriteLine($"Created: {f.CreationTime}");
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo d = new DirectoryInfo(path);
                    Console.WriteLine($"Directory: {d.Name}");
                    Console.WriteLine($"Created: {d.CreationTime}");
                }
                else
                {
                    Console.WriteLine("Path does not exist.");
                }
            });
        }
    }

    public class TimeCommand : ICommand
    {
        public string Name => "time";
        public string Description => "Show current system time";

        public Task Execute(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            return Task.CompletedTask;
        }
    }

    public class EchoCommand : ICommand
    {
        public string Name => "echo";
        public string Description => "Print text to console";

        public Task Execute(string[] args)
        {
            Console.WriteLine(string.Join(" ", args));
            return Task.CompletedTask;
        }
    }
}


