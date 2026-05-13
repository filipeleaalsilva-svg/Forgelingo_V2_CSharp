using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forgelingo.Core.AI
{
    // Simple skeleton. Implement provider specifics (DeepSeek, Gemini, OpenAI) here.
    public class AIEngineSkeleton : IAIEngine
    {
        private readonly string _apiKey;
        public AIEngineSkeleton(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Task<Dictionary<string, string>> TranslateBatchAsync(Dictionary<string, string> texts, object? context = null)
        {
            // Placeholder: implement calls to chosen AI provider, JSON parsing, error handling, caching and retries.
            // For now, return inputs (no-op) to allow pipeline testing.
            var result = new Dictionary<string, string>();
            foreach (var kv in texts) result[kv.Key] = kv.Value; // NO-OP
            return Task.FromResult(result);
        }

        public Task<string> RawCompleteAsync(string systemPrompt, string userPrompt)
        {
            throw new NotImplementedException("Implement provider-specific raw completion.");
        }
    }
}
