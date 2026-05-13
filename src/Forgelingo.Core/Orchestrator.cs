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

        public ForgelingoOrchestrator(IAIEngine ai)
        {
            _ai = ai;
        }

        public async Task ProcessJarAsync(string jarPath, string outputDir)
        {
            if (!File.Exists(jarPath)) throw new FileNotFoundException(jarPath);
            Directory.CreateDirectory(outputDir);

            using var jar = ZipFile.OpenRead(jarPath);
            var langEntries = jar.Entries.Where(e => e.FullName.EndsWith(".lang") || e.FullName.EndsWith(".json")).ToList();
            foreach (var entry in langEntries)
            {
                using var s = entry.Open();
                using var ms = new MemoryStream();
                await s.CopyToAsync(ms);
                ms.Position = 0;
                var text = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                if (entry.FullName.EndsWith(".lang"))
                {
                    var (structure, toTranslate) = Parsers.ParseLang(text);
                    // Call AI
                    var translated = await _ai.TranslateBatchAsync(toTranslate);
                    var rebuilt = Parsers.RebuildLang(structure, translated);
                    var outPath = Path.Combine(outputDir, Path.GetFileName(entry.FullName));
                    await File.WriteAllTextAsync(outPath, rebuilt);
                }
                else if (entry.FullName.EndsWith(".json"))
                {
                    // For simplicity copy json as-is (TODO: implement JSON extraction + translation)
                    var outPath = Path.Combine(outputDir, Path.GetFileName(entry.FullName));
                    await File.WriteAllTextAsync(outPath, text);
                }
            }
        }
    }
}
