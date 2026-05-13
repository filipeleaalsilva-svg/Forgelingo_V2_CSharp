using System.Collections.Generic;

namespace Forgelingo.Core
{
    public static partial class ModDatabase
    {
        // Extended glossary (subset ported from Python). Expand as needed.
        private static readonly Dictionary<string,string> Glossary = new()
        {
            {"Iron Ingot","Lingote de Ferro"},
            {"Gold Ingot","Lingote de Ouro"},
            {"Ingot","Lingote"},
            {"Crafting Table","Mesa de Trabalho"},
            {"Furnace","Fornalha"},
            {"Chest","Baú"},
            {"Bucket","Balde"},
            {"Pickaxe","Picareta"},
            {"Shovel","Pá"},
            {"Axe","Machado"},
            {"Sword","Espada"},
            {"Hoe","Enxada"},
            {"Smeltery","Fundição"},
            {"Casting","Moldagem"},
            {"Tool Part","Peça de Ferramenta"},
            {"Molten","Derretido"},
            {"Augment","Módulo"},
            {"Flux","Fluxo"},
            {"Coil","Bobina"},
            {"Mob","Mob"},
            {"Boss","Chefe"},
            {"Spawn","Spawn"},
            {"Loot","Loot"},
            {"Drop","Drop"},
            {"Hopper","Funil"},
            {"Dispenser","Dispensador"},
            {"Comparator","Comparador"},
            {"Repeater","Repetidor"},
            {"Rail","Trilho"},
            {"Beacon","Farol"},
            {"Tool","Ferramenta"},
            {"Recipe","Receita"},
        };

        public static IReadOnlyDictionary<string,string> GetGlossary() => Glossary;
    }
}
