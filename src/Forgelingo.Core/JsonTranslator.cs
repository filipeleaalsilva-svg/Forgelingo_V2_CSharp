using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Forgelingo.Core
{
    public static class JsonTranslator
    {
        private static readonly HashSet<string> BookTranslatableKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "text","title","name","description","label","subtext","tooltip","message","category","subtitle","header","footer","page_text","body","lore","pages","pages"
        };

        public static Dictionary<string,string> ExtractTexts(string json)
        {
            var result = new Dictionary<string,string>();
            if (string.IsNullOrWhiteSpace(json)) return result;
            var node = JsonNode.Parse(json);
            if (node is null) return result;
            Recurse(node, "", result);
            return result;
        }

        private static void Recurse(JsonNode node, string path, Dictionary<string,string> outDict)
        {
            if (node is JsonValue jv)
            {
                if (jv.TryGetValue<string>(out var sval))
                {
                    if (IsTranslatableKey(path)) outDict[path] = sval;
                }
                return;
            }

            if (node is JsonObject obj)
            {
                foreach (var kv in obj)
                {
                    var childPath = string.IsNullOrEmpty(path) ? kv.Key : path + "." + kv.Key;
                    if (kv.Value is JsonValue cv && cv.TryGetValue<string>(out var s) && BookTranslatableKeys.Contains(kv.Key.ToLowerInvariant()))
                    {
                        outDict[childPath] = s;
                    }
                    else
                    {
                        Recurse(kv.Value!, childPath, outDict);
                    }
                }
                return;
            }

            if (node is JsonArray arr)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    var childPath = path + "[" + i + "]";
                    var item = arr[i];
                    if (item is JsonValue iv && iv.TryGetValue<string>(out var s))
                    {
                        if (BookTranslatableKeys.Contains(GetLastPathSegment(path))) outDict[childPath] = s;
                    }
                    else
                    {
                        Recurse(item!, childPath, outDict);
                    }
                }
            }
        }

        private static string GetLastPathSegment(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var last = parts.Last();
            var idx = last.IndexOf('[');
            if (idx >= 0) last = last.Substring(0, idx);
            return last;
        }

        private static bool IsTranslatableKey(string path)
        {
            var last = GetLastPathSegment(path);
            return BookTranslatableKeys.Contains(last);
        }

        public static string ApplyTranslations(string json, Dictionary<string,string> translated)
        {
            if (string.IsNullOrWhiteSpace(json)) return json;
            var node = JsonNode.Parse(json);
            if (node is null) return json;
            ApplyRecurse(node, "", translated);
            return node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }

        private static void ApplyRecurse(JsonNode node, string path, Dictionary<string,string> translated)
        {
            if (node is JsonObject obj)
            {
                foreach (var kv in obj.ToList())
                {
                    var childPath = string.IsNullOrEmpty(path) ? kv.Key : path + "." + kv.Key;
                    if (kv.Value is JsonValue cv && cv.TryGetValue<string>(out var s) && BookTranslatableKeys.Contains(kv.Key.ToLowerInvariant()))
                    {
                        if (translated.TryGetValue(childPath, out var newv)) obj[kv.Key] = newv;
                    }
                    else
                    {
                        ApplyRecurse(kv.Value!, childPath, translated);
                    }
                }
                return;
            }

            if (node is JsonArray arr)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    var childPath = path + "[" + i + "]";
                    var item = arr[i];
                    if (item is JsonValue iv && iv.TryGetValue<string>(out var s))
                    {
                        if (translated.TryGetValue(childPath, out var newv)) arr[i] = newv;
                    }
                    else
                    {
                        ApplyRecurse(item!, childPath, translated);
                    }
                }
            }
        }
    }
}
