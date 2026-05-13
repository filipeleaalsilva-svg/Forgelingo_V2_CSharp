using System;
using System.IO;

namespace Forgelingo.UI
{
    public static class SettingsManager
    {
        private static string ConfigDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Forgelingo");
        private static string KeyFile => Path.Combine(ConfigDir, "apikey.txt");

        public static void SaveApiKey(string key)
        {
            Directory.CreateDirectory(ConfigDir);
            File.WriteAllText(KeyFile, key ?? "");
        }

        public static string LoadApiKey()
        {
            try { return File.Exists(KeyFile) ? File.ReadAllText(KeyFile) : string.Empty; }
            catch { return string.Empty; }
        }
    }
}
