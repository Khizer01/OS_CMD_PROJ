using System;
using System.Linq;
using System.Threading.Tasks;
using OS_CMD_PROJECT.Commands;
using OS_CMD_PROJECT.Services;

namespace OS_CMD_PROJECT
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the command registry (stores all available commands)
            var registry = new CommandRegistry();

            // Initialize the AI client (used for interacting with Gemini API)
            var aiClient = new GeminiClient();

            // Register commands
            registry.Register(new HelpCommand(registry)); // Command to list all available commands
            registry.Register(new TaskCommands()); // Command to list running tasks (processes)
            registry.Register(new OpenProcessCommand()); // Command to open a process using it's id
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
            registry.Register(new TimeCommand());// Command to display current system time
            registry.Register(new EchoCommand());// Command to print text to the console
            registry.Register(new FileInfoCommand()); // Command to show detailed file information
            registry.Register(new DirectoryInfoCommand()); // Command to show detailed directory information
            registry.Register(new MonitorProcessCommand());//command to monitor process memory continously
            registry.Register(new ExitCommand()); // Command to exit the CLI
            registry.Register(new UndoCommand()); // Command to undo the last action
            registry.Register(new RedoCommand()); // Command to redo the last undone action
            // Display a welcome message
            Console.WriteLine("Welcome to OS CLI\nType 'help' to see available commands. Type 'exit' to quit.");


            //LOCAL SUGGESTION:
            //1. handle input async
            //2. execute command async
            //3. handle unknown command async
            //4. levenshtein distance function 



            while (true)
            {
                Console.Write("\n>> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                await HandleInputAsync(input, registry, aiClient);
            }

            Console.WriteLine("Goodbye!");
        }

       
       
        // ----------------- INPUT HANDLER -----------------
        private static async Task HandleInputAsync(
            string input,
            CommandRegistry registry,
            GeminiClient aiClient)
        {
            var parts = input.Trim().Split(' ');
            var commandName = parts[0];
            var commandArgs = parts.Skip(1).ToArray();

            var command = registry.Get(commandName);

            if (command != null)
            {
                await ExecuteCommandAsync(command, commandArgs);
            }
            else
            {
                await HandleUnknownCommandAsync(commandName, registry, aiClient);
            }
        }

        // ----------------- EXECUTE COMMAND -----------------
        private static async Task ExecuteCommandAsync(
            ICommand command,
            string[] args)
        {
            try
            {
                await command.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // ----------------- UNKNOWN COMMAND -----------------
        private static async Task HandleUnknownCommandAsync(
            string commandName,
            CommandRegistry registry,
            GeminiClient aiClient)
        {
            Console.WriteLine("Command not found.");

            var suggestions = registry.GetAll()
                .Select(c => new
                {
                    Command = c,
                    Score = CalculateSimilarityScore(commandName, c.Name)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(3)
                .Select(x => x.Command.Name)
                .ToList();

            if (suggestions.Any())
            {
                Console.WriteLine("Did you mean:");
                foreach (var suggestion in suggestions)
                    Console.WriteLine($" - {suggestion}");
            }
            else
            {
                await AskAiForSuggestionAsync(commandName, registry, aiClient);
            }
        }

        // ----------------- AI SUGGESTION -----------------
        private static async Task AskAiForSuggestionAsync(
            string commandName,
            CommandRegistry registry,
            GeminiClient aiClient)
        {
            Console.WriteLine("No local suggestion found. Asking AI...");

            string availableCommands = string.Join("\n",
                registry.GetAll().Select(c => $"{c.Name} - {c.Description}"));

            string prompt =
                $"I typed a command '{commandName}' which does not exist.\n" +
                $"Here is the list of available commands:\n{availableCommands}\n" +
        $"Please suggest the **single most likely command** I intended. **Return only the command name, no explanation.**";

            try
            {
                var response = await aiClient.QueryAsync(prompt);
                Console.WriteLine("\nAI Suggestion:");
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Error: {ex.Message}");
            }
        }
        

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t?.Length ?? 0;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }
            return d[s.Length, t.Length];
        }

        public static int CalculateSimilarityScore(string input, string command)
        {
            input = input.ToLower();
            command = command.ToLower();
            int score = 0;

            if (command == input) return 1000;
            
            if (command.StartsWith(input))
            {
                score += 100;
                int lengthRatio = (input.Length * 100) / command.Length;
                score += lengthRatio;
            }
            
            var inputTokens = input.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var commandTokens = command.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            int tokenMatchScore = 0;
            bool hasTokenStartMatch = false;
            int matchingTokenCount = 0;
            
            foreach (var inputToken in inputTokens)
            {
                if (inputToken.Length < 2) continue;
                
                bool foundMatch = false;
                foreach (var cmdToken in commandTokens)
                {
                    if (cmdToken == inputToken)
                    {
                        tokenMatchScore += 50;
                        hasTokenStartMatch = true;
                        foundMatch = true;
                        break;
                    }
                    else if (cmdToken.StartsWith(inputToken))
                    {
                        int ratio = (inputToken.Length * 100) / cmdToken.Length;
                        tokenMatchScore += Math.Min(40, ratio / 2);
                        hasTokenStartMatch = true;
                        foundMatch = true;
                        break;
                    }
                    else if (cmdToken.Contains(inputToken) && inputToken.Length >= 3)
                    {
                        tokenMatchScore += 10;
                        foundMatch = true;
                    }
                }
                if (foundMatch) matchingTokenCount++;
            }
            score += tokenMatchScore;

            if (hasTokenStartMatch && commandTokens.Length > 1)
            {
                int preferShorterCommandBonus = (3 - Math.Min(2, commandTokens.Length - 1)) * 5;
                score += preferShorterCommandBonus;
            }

            if (command.Contains(input) && !command.StartsWith(input) && !hasTokenStartMatch)
            {
                int position = command.IndexOf(input);
                int positionPenalty = position * 2;
                score += Math.Max(5, 30 - positionPenalty);
            }

            int distance = LevenshteinDistance(input, command);
            if (distance <= 2 && input.Length >= 3)
            {
                int distanceScore = (3 - distance) * 15;
                int lengthPenalty = Math.Abs(input.Length - command.Length) * 2;
                score += Math.Max(0, distanceScore - lengthPenalty);
            }
            else if (distance == 1 && input.Length >= 2)
            {
                int distanceScore = 20;
                int lengthPenalty = Math.Abs(input.Length - command.Length) * 3;
                score += Math.Max(0, distanceScore - lengthPenalty);
            }

            return score;
        }
    }
}


