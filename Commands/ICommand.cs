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

            string filePath=args[0];
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

    public class DeleteDreectoryCommand : ICommand
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

    public class Date : ICommand {
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
            string date = DateTime.Now.ToString("F");

            using (var fs = File.Create(date))
            {
                await fs.FlushAsync(); // ensure async creation
            }

            Console.WriteLine(date);
        }

    }
            string date = DateTime.Now.ToString("F");

            using (var fs = File.Create(date))
            {
                await fs.FlushAsync(); // ensure async creation
            }

            Console.WriteLine(date);
        }

    }
}
