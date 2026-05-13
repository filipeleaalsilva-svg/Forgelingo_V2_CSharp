using System.Collections.Generic;

namespace Forgelingo.Core
{
    public static class ModDatabase
    {
        public static (string label, string style, string examples) GetToneProfile(string category)
        {
            // minimal profiles; expand as needed
            return category switch
            {
                "magic" => ("Magia/Arcano", "místico, poético e imersivo.", "'ritual' -> 'ritual'"),
                "tech" => ("Tecnológico/Industrial", "técnico, direto e didático.", "'generator' -> 'gerador'"),
                _ => ("Genérico", "natural, claro e idiomático.", "")
            };
        }

        private static readonly Dictionary<string,string> Map = new()
        {
            {"tinkersconstruct","tinkers"},
            {"botania","magic"},
            {"thermalexpansion","tech"},
            {"immersiveengineering","tech"}
        };

        public static string GetCategory(string? modId)
        {
            if (string.IsNullOrEmpty(modId)) return "unknown";
            return Map.TryGetValue(modId.ToLowerInvariant(), out var c) ? c : "unknown";
        }
    }
}
