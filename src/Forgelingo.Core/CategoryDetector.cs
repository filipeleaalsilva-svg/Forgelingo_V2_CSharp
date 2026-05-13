using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Forgelingo.Core
{
    public static class CategoryDetector
    {
        private static readonly Dictionary<string, (string[] keywords, double weight)> Indicators =
            new()
            {
                { "magic", (new[] {"ritual","mana","spell","arcane","essence","enchantment","rune","grimoire","conjure"}, 2.0) },
                { "tech", (new[] {"generator","energy","tier","rf","flux","multiblock","circuit","input","output","machine"}, 1.8) },
                { "storage", (new[] {"storage","autocrafting","network","cell","terminal","channel","drive"}, 2.0) },
                { "farming", (new[] {"seed","crop","plant","harvest","soil","fertilizer","farm"}, 1.9) },
                { "adventure", (new[] {"dungeon","treasure","ruins","beast","loot","quest","explore"}, 1.7) },
                { "tinkers", (new[] {"smeltery","casting","tool part","modifier","anvil","forge","ingot"}, 2.1) },
                { "dark", (new[] {"corrupt","abyss","whisper","dark","shadow","soul","curse","eldritch"}, 1.8) }
            };

        public static (string category, double confidence) DetectCategoryFromContent(object data)
        {
            if (data == null) return ("unknown", 0.0);
            var text = data.ToString()!.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(text) || text.Length < 100) return ("unknown", 0.0);

            var scores = new Dictionary<string, double>();
            foreach (var kv in Indicators)
            {
                double score = 0;
                foreach (var keyword in kv.Value.keywords)
                {
                    // simple word boundary search
                    var pattern = $"\\b{Regex.Escape(keyword)}\\b";
                    var matches = Regex.Matches(text, pattern).Count;
                    score += matches * kv.Value.weight;
                }
                scores[kv.Key] = score;
            }

            var maxScore = scores.Values.DefaultIfEmpty(0).Max();
            if (maxScore <= 0) return ("unknown", 0.0);

            var detected = scores.Aggregate((l, r) => l.Value >= r.Value ? l : r).Key;
            // normalize confidence by heuristic
            var confidence = Math.Min(maxScore / 10.0, 1.0);
            confidence *= Math.Min(text.Length / 1000.0, 1.0);

            return (detected, Math.Round(confidence, 2));
        }
    }
}
