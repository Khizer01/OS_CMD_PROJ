using System.Threading.Tasks;

namespace OS_CMD_PROJECT.Services
{
    // Interface for AI client services
    // Defines a contract for querying an AI system
    public interface IAIClient
    {
        // Sends a query to the AI and gets a response
        Task<string> QueryAsync(string prompt);
    }
}
