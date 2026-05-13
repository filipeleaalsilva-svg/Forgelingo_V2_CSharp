using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Forgelingo.Core
{
    public static class ContextDetector
    {
        // Uses CategoryDetector to produce context object
        public static Dictionary<string, object?> BuildContext(string filePath, string fileKind, object? data=null, string? modNameHint = null)
        {
            var modId = DetectModId(filePath);
            var (category, confidence) = CategoryDetector.DetectCategoryFromContent(data ?? string.Empty);
            if (confidence < 0.5)
            {
                // fallback to mod database
                category = ModDatabase.GetCategory(modId);
                confidence = 0.8;
            }

            var tone = ModDatabase.GetToneProfile(category);
            var bookType = fileKind == "lang" ? "lang" : DetectBookType(filePath, data);

            return new Dictionary<string, object?>
            {
                ["mod_id"] = modId,
                ["category"] = category,
                ["category_confidence"] = confidence,
                ["category_label"] = tone.label,
                ["category_style"] = tone.style,
                ["book_type"] = bookType,
                ["file_path"] = filePath,
            };
        }

        public static string? DetectModId(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            var parts = filePath.Replace("\\","/").ToLowerInvariant().Split('/');
            for (int i = 0; i < parts.Length - 1; i++) if (parts[i] == "assets") return parts[i+1];
            return null;
        }

        private static string DetectBookType(string filePath, object? data)
        {
            // basic heuristics
            var p = filePath.ToLowerInvariant();
            if (p.Contains("patchouli")) return "patchouli";
            if (p.Contains("ftbquests") || p.Contains("betterquesting")) return "ftb_questbook";
            if (p.Contains("/manual") || p.Contains("/guide")) return "manual";
            return "generic_book";
        }
    }
}
