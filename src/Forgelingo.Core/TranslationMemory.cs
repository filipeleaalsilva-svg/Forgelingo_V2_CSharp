using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Forgelingo.Core
{
    public class TranslationMemory
    {
        private readonly ConcurrentDictionary<string,string> _mem = new();
        private readonly string _path;

        public TranslationMemory(string? storagePath = null)
        {
            var dir = storagePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Forgelingo");
            Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, "translation_memory.json");
            Load();
        }

        private void Load()
        {
            if (!File.Exists(_path)) return;
            try
            {
                var json = File.ReadAllText(_path);
                var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(json);
                if (dict != null) foreach (var kv in dict) _mem[kv.Key] = kv.Value;
            }
            catch { /* ignore */ }
        }

        public void Save()
        {
            try
            {
                var dict = new Dictionary<string,string>(_mem);
                File.WriteAllText(_path, JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        public bool TryGet(string original, out string translated) => _mem.TryGetValue(original, out translated);

        public void Add(string original, string translated) => _mem[original] = translated;
    }
}
