using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace Forgelingo.Tools
{
    // Simple aggregator: fetch a set of language files (PO, key=value, simple JSON),
    // normalize and output aggregated glossary (english -> preferred pt-BR).
    public static class GlossaryAggregator
    {
        private static readonly HttpClient _http = new HttpClient();

        public static async Task RunAsync(IEnumerable<string> urls, string outPath)
        {
            var candidates = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);

            foreach (var url in urls)
            {
                try
                {
                    var txt = await _http.GetStringAsync(url);
                    var pairs = ParsePossibleLangFile(txt);
                    foreach (var kv in pairs)
                    {
                        var key = kv.Key.Trim();
                        var val = kv.Value.Trim();
                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val))
                            continue;

                        if (!candidates.TryGetValue(key, out var freq))
                        {
                            freq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                            candidates[key] = freq;
                        }
                        if (!freq.ContainsKey(val)) freq[val] = 0;
                        freq[val]++;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to fetch {url}: {ex.Message}");
                }
            }

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in candidates)
            {
                // pick most frequent translation (community consensus)
                var best = "";
                var bestCount = -1;
                foreach (var t in kv.Value)
                {
                    if (t.Value > bestCount)
                    {
                        best = t.Key;
                        bestCount = t.Value;
                    }
                }
                // preserve placeholders like {0}, %s, etc. If best is identical to key, skip.
                if (!string.Equals(best, kv.Key, StringComparison.OrdinalIgnoreCase))
                    result[kv.Key] = best;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? ".");
            await File.WriteAllTextAsync(outPath, JsonSerializer.Serialize(result, new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine($"Aggregated {result.Count} terms -> {outPath}");
        }

        private static Dictionary<string,string> ParsePossibleLangFile(string txt)
        {
            // Try PO (msgid/msgstr)
            if (txt.Contains("msgid") && txt.Contains("msgstr"))
            {
                return ParsePo(txt);
            }
            // Try simple key=value lines
            if (txt.Contains("=") && !txt.TrimStart().StartsWith("{"))
            {
                return ParseKeyEquals(txt);
            }
            // Try JSON-like
            if (txt.TrimStart().StartsWith("{"))
            {
                try
                {
                    var doc = JsonSerializer.Deserialize<Dictionary<string,string>>(txt);
                    return doc ?? new Dictionary<string,string>();
                }
                catch {}
            }

            return new Dictionary<string,string>();
        }

        private static Dictionary<string,string> ParsePo(string txt)
        {
            var dict = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            var msgidRx = new Regex("msgid\s+\"(?<id>.*?)\"", RegexOptions.Singleline);
            var msgstrRx = new Regex("msgstr\s+\"(?<str>.*?)\"", RegexOptions.Singleline);

            var idMatches = msgidRx.Matches(txt);
            var strMatches = msgstrRx.Matches(txt);
            var n = Math.Min(idMatches.Count, strMatches.Count);
            for (int i=0;i<n;i++)
            {
                var id = idMatches[i].Groups["id"].Value;
                var st = strMatches[i].Groups["str"].Value;
                if (!string.IsNullOrWhiteSpace(id)) dict[id] = st;
            }
            return dict;
        }

        private static Dictionary<string,string> ParseKeyEquals(string txt)
        {
            var dict = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            using (var sr = new StringReader(txt))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//")) continue;
                    var idx = line.IndexOf('=');
                    if (idx>0)
                    {
                        var k = line.Substring(0, idx).Trim();
                        var v = line.Substring(idx+1).Trim();
                        dict[k] = v;
                    }
                }
            }
            return dict;
        }
    }
}
