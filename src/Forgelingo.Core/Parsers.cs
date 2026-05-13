using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Forgelingo.Core
{
    public static class Parsers
    {
        private static readonly Regex LangLineRe = new(@"^([^#=\s][^=]*?)=(.*)$", RegexOptions.Compiled);

        public static (List<(string kind, string content)> structure, Dictionary<string,string> toTranslate) ParseLang(string content, bool keepNames=true)
        {
            var structure = new List<(string, string)>();
            var toTranslate = new Dictionary<string,string>();
            using var reader = new System.IO.StringReader(content ?? "");
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    structure.Add(("KEEP", line + "\n"));
                    continue;
                }
                var m = LangLineRe.Match(line);
                if (m.Success)
                {
                    var key = m.Groups[1].Value.Trim();
                    var value = m.Groups[2].Value;
                    // very basic rule
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        toTranslate[key] = value;
                        structure.Add(("TRANSLATE", key));
                    }
                    else
                        structure.Add(("KEEP", line + "\n"));
                }
                else
                {
                    structure.Add(("KEEP", line + "\n"));
                }
            }
            return (structure, toTranslate);
        }

        public static string RebuildLang(List<(string kind,string content)> structure, Dictionary<string,string> translated)
        {
            var sb = new StringBuilder();
            foreach (var item in structure)
            {
                if (item.kind == "KEEP") sb.Append(item.content);
                else if (item.kind == "TRANSLATE")
                {
                    var key = item.content;
                    if (translated.TryGetValue(key, out var val)) sb.Append(key + "=" + val + "\n");
                    else sb.Append(key + "=" + "\n");
                }
            }
            return sb.ToString();
        }
    }
}
