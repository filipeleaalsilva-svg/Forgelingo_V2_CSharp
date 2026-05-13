using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Forgelingo.Core.AI
{
    /// <summary>
    /// DeepSeek-compatible AI engine using HTTP calls to the provider's chat completions endpoint.
    /// Configure via environment variable DEEPSEEK_API_KEY and optional DEEPSEEK_BASE_URL.
    /// </summary>
    public class DeepSeekAIEngine : IAIEngine, IDisposable
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
        private bool _disposed = false;

        public DeepSeekAIEngine(string apiKey, string? baseUrl = null, HttpMessageHandler? handler = null)
        {
            _baseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "https://api.deepseek.com" : baseUrl.TrimEnd('/');
            _http = handler is null ? new HttpClient() : new HttpClient(handler);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("Forgelingo/1.0");
            _http.Timeout = TimeSpan.FromSeconds(60);
        }

        public async Task<Dictionary<string, string>> TranslateBatchAsync(Dictionary<string, string> texts, object? context = null)
        {
            if (texts == null || texts.Count == 0) return new Dictionary<string, string>();

            // Build a conservative system prompt to preserve placeholders and JSON structure
            var systemPrompt = new StringBuilder();
            systemPrompt.AppendLine("You are a translation assistant that converts a JSON object of key->value strings from English to Brazilian Portuguese (pt-BR).\n");
            systemPrompt.AppendLine("Rules:");
            systemPrompt.AppendLine("- Only translate values, do not change keys.");
            systemPrompt.AppendLine("- Preserve placeholders exactly: %s,%d,%x, {0}, {1}, $(item), $(br), §x, &x, [v1], <br>, \\n.");
            systemPrompt.AppendLine("- Return strictly a valid JSON object mapping the same keys to translated strings.");
            systemPrompt.AppendLine("- Do not add commentary or any wrapper text. Respond ONLY with the JSON object.");

            var userPrompt = JsonSerializer.Serialize(texts, _jsonOptions);

            var payload = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt.ToString() },
                    new { role = "user", content = userPrompt }
                },
                response_format = new { type = "json_object" },
                temperature = 0.0
            };

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var respText = await PostWithRetriesAsync($"{_baseUrl}/v1/chat/completions", json);

            // clean fences if present
            respText = CleanMarkdown(respText);

            try
            {
                var doc = JsonDocument.Parse(respText);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    var outDict = new Dictionary<string, string>();
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        outDict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                    }
                    return outDict;
                }
            }
            catch (JsonException)
            {
                // try to extract JSON using regex
                var m = Regex.Match(respText, "\{[\s\S]*\}");
                if (m.Success)
                {
                    try
                    {
                        var doc2 = JsonDocument.Parse(m.Value);
                        var outDict = new Dictionary<string, string>();
                        foreach (var prop in doc2.RootElement.EnumerateObject()) outDict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                        return outDict;
                    }
                    catch { /* fallthrough */ }
                }
            }

            // If parsing failed, as a safe fallback return originals
            var fallback = new Dictionary<string, string>();
            foreach (var kv in texts) fallback[kv.Key] = kv.Value;
            return fallback;
        }

        public async Task<string> RawCompleteAsync(string systemPrompt, string userPrompt)
        {
            var payload = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.2
            };
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var respText = await PostWithRetriesAsync($"{_baseUrl}/v1/chat/completions", json);
            return CleanMarkdown(respText);
        }

        private static string CleanMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
            // remove ```json or ``` fences
            text = Regex.Replace(text, "^```json\\s*", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^```\\s*", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "```$", "", RegexOptions.IgnoreCase);
            return text.Trim();
        }

        private async Task<string> PostWithRetriesAsync(string url, string jsonBody)
        {
            int maxRetries = 3;
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Exception? lastEx = null;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using var resp = await _http.PostAsync(url, content);
                    var text = await resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode) return text;
                    lastEx = new HttpRequestException($"Status {(int)resp.StatusCode}: {text}");
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                }
                await Task.Delay((int)(Math.Pow(2, attempt) * 250));
            }
            throw lastEx ?? new Exception("Unknown HTTP error");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _http.Dispose();
                _disposed = true;
            }
        }
    }
}
