using System;
using System.Threading.Tasks;
using OS_CMD_PROJECT.Services;

namespace OS_CMD_PROJECT.Commands
{
    // Command to interact with the AI system
    public class AskAiCommand : ICommand
    {
        private readonly IAIClient _aiClient; // Reference to the AI client

        // Constructor takes an AI client to send queries
        public AskAiCommand(IAIClient aiClient)
        {
            _aiClient = aiClient ?? throw new ArgumentNullException(nameof(aiClient));
        }

        public string Name => "ask-ai"; // Command name
        public string Description => "Ask the AI a question or start a chat. Usage: ask-ai <question> | chat-ai";

        public async Task Execute(string[] args)
        {
            // Ensure arguments are provided
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Usage: ask-ai <question> OR ask-ai chat-ai");
                return;
            }

            string firstArg = args[0].ToLower();

            if (firstArg == "chat-ai")
            {
                await StartChatAsync();
                return;
            }

            var prompt = string.Join(" ", args);
            Console.WriteLine("Sending to AI...");

            try
            {
                var response = await _aiClient.QueryAsync(prompt);
                Console.WriteLine("AI response:\n" + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying AI: " + ex.Message);
            }
        }

        private async Task StartChatAsync()
        {
            Console.WriteLine("Starting AI Chat session. Type 'exit' to end the conversation.");
            Console.WriteLine("----------------------------------------");

            string userInput;
            do
            {
                Console.Write("You: ");
                userInput = Console.ReadLine();

                if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
                    break;

                try
                {
                    string response = await _aiClient.QueryAsync(userInput);
                    Console.WriteLine("\nAI: " + response);
                    Console.WriteLine("----------------------------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error communicating with AI: {ex.Message}");
                }

            } while (true);

            Console.WriteLine("AI chat session ended.");
        }
    }
}


// AI Client
public class AIClient : IAIClient
{
    private readonly string _apiKey;

    public AIClient()
    {
        _apiKey = Environment.GetEnvironmentVariable("AI_API_KEY") ?? "YOUR_API_KEY";

        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new Exception("AI API Key not found.");
    }

    // Single query
    public async Task<string> QueryAsync(string prompt)
    {
        // Simulate API call (replace with actual API call)
        await Task.Delay(200);
        return $"You asked: {prompt}";
    }
}
