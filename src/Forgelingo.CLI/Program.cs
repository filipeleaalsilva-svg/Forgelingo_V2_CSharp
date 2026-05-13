using System;
using Forgelingo.Core;

namespace Forgelingo.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Forgelingo V2 - C# skeleton\n");

            // quick demo of category detector
            var sample = "The Smeltery is a multiblock forge used to cast tool parts and ingots. Use molten metal.";
            var (category, confidence) = CategoryDetector.DetectCategoryFromContent(sample);
            Console.WriteLine($"Detected category: {category} (confidence: {confidence:P0})");

            Console.WriteLine("\nParse demo:\n");
            var lang = "item.example.name=Example Item\nitem.example.desc=An example item for testing\n";
            var (structure, toTranslate) = Parsers.ParseLang(lang);
            Console.WriteLine("To translate keys:");
            foreach (var k in toTranslate.Keys) Console.WriteLine(" - " + k);
        }
    }
}
