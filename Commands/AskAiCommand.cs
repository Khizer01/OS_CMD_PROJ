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
            _aiClient = aiClient;
        }

        public string Name => "ask-ai"; // Command name
        public string Description => "Ask the AI a question. Usage: ask-ai <your question>"; // Command description

        public async Task Execute(string[] args)
        {
            // Ensure the user provides a question
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Usage: ask-ai <question>");
                return;
            }

            // Combine arguments into a single question
            var prompt = string.Join(" ", args);
            Console.WriteLine("Sending to AI...");

            // Get the AI's response
            var response = await _aiClient.QueryAsync(prompt);
            Console.WriteLine("AI response:\n" + response);
        }
    }
}
