using System;
using System.Collections.Generic;

namespace Forgelingo.Core
{
    public static class ModDatabase
    {
        // Glossary: English -> Preferred Portuguese translation (pt-BR)
        // Curated from existing project terms, community glossaries and common mod terminology.
        public static readonly IReadOnlyDictionary<string, string> Glossary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // General Minecraft
            ["block"] = "bloco",
            ["item"] = "item",
            ["entity"] = "entidade",
            ["biome"] = "bioma",
            ["dimension"] = "dimensão",
            ["overworld"] = "Sobremundo",
            ["nether"] = "Inferno",
            ["end"] = "Fim",
            ["spawn"] = "ponto de surgimento",
            ["tooltip"] = "dica",

            // UI / common
            ["settings"] = "configurações",
            ["inventory"] = "inventário",
            ["health"] = "vida",
            ["mana"] = "mana",
            ["durability"] = "durabilidade",

            // Gameplay terms
            ["smelt"] = "fundir",
            ["smelting"] = "fusão",
            ["ore"] = "minério",
            ["ingot"] = "lingote",
            ["nugget"] = "pepita",
            ["craft"] = "fabricar",
            ["crafting"] = "fabricação",

            // Redstone and tech
            ["redstone"] = "redstone",
            ["circuit"] = "circuito",
            ["energy"] = "energia",
            ["rf"] = "RF",
            ["fe"] = "FE",

            // Mods / common modded terms
            ["smeltery"] = "fundição",
            ["tinkers" ] = "Tinkers' Construct",
            ["grinder"] = "moedor",
            ["alloy"] = "liga",
            ["multiblock"] = "multibloco",
            ["controller"] = "controlador",
            ["conduit"] = "condutor",

            // Magic mods
            ["ritual"] = "ritual",
            ["mana pool"] = "reserva de mana",

            // Book / patchouli specific
            ["entry"] = "entrada",
            ["category"] = "categoria",
            ["author"] = "autor",
            ["pages"] = "páginas",
            ["spotlight"] = "destaque",

            // Machine / automation
            ["assembler"] = "montador",
            ["crusher"] = "triturador",
            ["press"] = "prensa",
            ["furnace"] = "forno",

            // Common items
            ["pickaxe"] = "picareta",
            ["axe"] = "machado",
            ["shovel"] = "pá",
            ["sword"] = "espada",
            ["bow"] = "arco",

            // Containers and storage
            ["chest"] = "baú",
            ["barrel"] = "barril",
            ["crate"] = "caixa",

            // Transportation
            ["boat"] = "barco",
            ["minecart"] = "vagão",

            // Technical UI hints
            ["click to open"] = "clique para abrir",
            ["right click"] = "clique com o botão direito",
            ["shift click"] = "shift+clique",

            // Placeholder examples (keep unchanged when translating)
            ["{0}"] = "{0}",
            ["%s"] = "%s",

            // Expanded common mod terms (aggregated)
            ["charging station"] = "estação de carregamento",
            ["flux"] = "fluxo",
            ["capacitor"] = "capacitor",
            ["wire"] = "fio",
            ["condenser"] = "condensador",
            ["alchemy"] = "alquimia",
            ["ritual stone"] = "pedra ritual",
            ["essence"] = "essência",

            // AE2 / storage networks
            ["network"] = "rede",
            ["controller"] = "controlador",
            ["terminal"] = "terminal",

            // Thermal/Gen mods
            ["smelter"] = "fundidor",
            ["reactor"] = "reator",

            // Misc gameplay nouns
            ["quest"] = "missão",
            ["recipe"] = "receita",
            ["progress"] = "progresso",

            // Safe defaults (when unsure)
            ["unknown"] = "desconhecido",

            // Add more domain-specific mappings as the glossary grows...
        };

        // Tone profiles or mod-specific overrides could be added here.
        public static readonly IReadOnlyDictionary<string, Dictionary<string, string>> ModSpecificGlossaries =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            // Example: mod id -> glossary overrides
            ["tconstruct"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["smeltery"] = "fundição",
                ["casting table"] = "mesa de fundição"
            }
        };
    }
}
