using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Forgelingo.Core
{
    public class TranslationComparator
    {
        // Improved placeholder regex covering %, {n}, $(...), [v1], § and & color codes, HTML tags
        private static readonly Regex PlaceholderRegex = new(
            @"(%(?:\d+\$)?[a-zA-Zxd]|%%|{\d+(?::\d+)?}|\$\([a-zA-Z0-9:_-]+\)|\[(?:v\d+|text|item|br|img|gui)\]|&[0-9a-fk-orxnm]|§[0-9a-fk-or]|\\\\n|</?[a-zA-Z]+[^>]*>)",
            RegexOptions.Compiled);

        public static IEnumerable<string> ExtractPlaceholders(string? text)
        {
            if (string.IsNullOrEmpty(text)) yield break;
            foreach (Match m in PlaceholderRegex.Matches(text)) yield return m.Value;
        }

        // Determines if a translation needs correction and a reason
        public static (bool needsCorrection, string reason) NeedsCorrection(string original, string translated)
        {
            if (string.IsNullOrWhiteSpace(translated)) return (true, "vazio ou inválido");

            var origPlaceholders = ExtractPlaceholders(original).OrderBy(s => s).ToArray();
            var transPlaceholders = ExtractPlaceholders(translated).OrderBy(s => s).ToArray();

            if (!origPlaceholders.SequenceEqual(transPlaceholders))
            {
                var missing = string.Join(", ", origPlaceholders.Except(transPlaceholders));
                var extra = string.Join(", ", transPlaceholders.Except(origPlaceholders));
                var msg = "placeholders alterados";
                if (!string.IsNullOrEmpty(missing)) msg += $"; faltam: {missing}";
                if (!string.IsNullOrEmpty(extra)) msg += $"; extras: {extra}";
                return (true, msg);
            }

            if (original == translated && Regex.IsMatch(original, @"[A-Za-z]"))
                return (true, "não traduzido (idêntico ao original)");

            // detect residual English words
            var englishIndicators = new HashSet<string> { "the","and","or","of","for","to","in","on","is","are","you","this","that","will" };
            var words = Regex.Matches(translated.ToLowerInvariant(), "\\b[a-z]+\\b").Select(m => m.Value).ToArray();
            if (words.Length > 5)
            {
                var englishCount = words.Count(w => englishIndicators.Contains(w));
                if ((double)englishCount / words.Length > 0.3) return (true, $"muitas palavras em inglês ({englishCount}/{words.Length})");
            }

            // length heuristic
            var ratio = (double)translated.Length / Math.Max(original.Length, 1);
            if (ratio > 1.8 && !Regex.IsMatch(original, "%|\{|\$|\[|<"))
                return (true, $"comprimento suspeito (original: {original.Length}, tradução: {translated.Length})");

            return (false, string.Empty);
        }
    }
}
