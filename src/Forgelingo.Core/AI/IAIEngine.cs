using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forgelingo.Core.AI
{
    public interface IAIEngine
    {
        // Translates a batch of texts: key => original
        Task<Dictionary<string, string>> TranslateBatchAsync(Dictionary<string,string> texts, object? context = null);

        // Raw completion for web research / profile extraction
        Task<string> RawCompleteAsync(string systemPrompt, string userPrompt);
    }
}
