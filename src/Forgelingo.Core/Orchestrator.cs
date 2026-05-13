using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Forgelingo.Core.AI;

namespace Forgelingo.Core
{
    public class ForgelingoOrchestrator
    {
        private readonly IAIEngine _ai;
        private readonly TranslationMemory _memory;

        public ForgelingoOrchestrator(IAIEngine ai, TranslationMemory? memory = null)
        {
            _ai = ai;
            _memory = memory ?? new TranslationMemory();
        }

        public async Task ProcessJarAsync(string jarPath, string outputDir)
        {
            if (!File.Exists(jarPath)) throw new FileNotFoundException(jarPath);
            Directory.CreateDirectory(outputDir);

            using var jar = ZipFile.OpenRead(jarPath);
            var entries = jar.Entries.Where(e => e.FullName.EndsWith(".lang") || e.FullName.EndsWith(".json")).ToList();
            foreach (var entry in entries)
            {
                using var s = entry.Open();
                using var ms = new MemoryStream();
                await s.CopyToAsync(ms);
                ms.Position = 0;
                var text = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                if (entry.FullName.EndsWith(".lang"))
                {
                    var (structure, toTranslate) = Parsers.ParseLang(text);
                    var pending = new Dictionary<string,string>();
                    var prefilled = new Dictionary<string,string>();
                    foreach (var kv in toTranslate)
                    {
                        if (_memory.TryGet(kv.Value, out var m)) prefilled[kv.Key] = m;
                        else pending[kv.Key] = kv.Value;
                    }
                    Dictionary<string,string> translated = new();
                    if (pending.Count > 0) translated = await _ai.TranslateBatchAsync(pending, context: null);
                    // combine
                    var final = new Dictionary<string,string>(prefilled);
                    foreach (var kv in translated) final[kv.Key] = kv.Value;
                    foreach (var kv in final) _memory.Add(toTranslate[kv.Key], kv.Value);

                    var rebuilt = Parsers.RebuildLang(structure, final);
                    var outPath = Path.Combine(outputDir, Path.GetFileName(entry.FullName));
                    await File.WriteAllTextAsync(outPath, rebuilt);
                }
                else if (entry.FullName.EndsWith(".json"))
                {
                    // JSON extraction and translation
                    var extracted = JsonTranslator.ExtractTexts(text);
                    var pending = new Dictionary<string,string>();
                    var prefilled = new Dictionary<string,string>();
                    foreach (var kv in extracted)
                    {
                        if (_memory.TryGet(kv.Value, out var mem)) prefilled[kv.Key] = mem;
                        else pending[kv.Key] = kv.Value;
                    }

                    Dictionary<string,string> translated = new();
                    if (pending.Count > 0)
                    {
                        // translate by sending mapping key->value, AI should return same keys
                        translated = await _ai.TranslateBatchAsync(pending, context: null);
                    }

                    var final = new Dictionary<string,string>(prefilled);
                    foreach (var kv in translated) final[kv.Key] = kv.Value;
                    // update memory using original values
                    foreach (var kv in final)
                    {
                        var orig = extracted[kv.Key];
                        _memory.Add(orig, kv.Value);
                    }

                    var applied = JsonTranslator.ApplyTranslations(text, final);
                    var outPath = Path.Combine(outputDir, Path.GetFileName(entry.FullName));
                    await File.WriteAllTextAsync(outPath, applied);
                }
            }

            _memory.Save();
        }
    }
}
