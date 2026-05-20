using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.RiftArsenal;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Equips.ScepterAccessories;

namespace DestroyerTest.Common
{
    public static class DTCrossMod
    {
        public const string FablesName = "CalamityFables";
        public static bool FablesIsLoaded;
        public static Mod FablesMod;

        public const string OrionName = "ConstellationsofOrion";
        public static bool OrionIsLoaded;
        public static Mod OrionMod;

        public const string FargosSoulsName = "FargowiltasSouls";
        public static bool FargosSoulsIsLoaded;
        public static Mod FargosSoulsMod;

        public const string FargosMutantName = "Fargowiltas";
        public static bool FargosMutantIsLoaded;
        public static Mod FargosMutantMod;

        public static void LoadMods()
        {
            FablesIsLoaded = ModLoader.TryGetMod(FablesName, out Mod fables);
            FablesMod = fables;
            OrionIsLoaded = ModLoader.TryGetMod(OrionName, out Mod orion);
            OrionMod = orion;
            FargosSoulsIsLoaded = ModLoader.TryGetMod(FargosSoulsName, out Mod frgoS);
            FargosSoulsMod = frgoS;
            FargosMutantIsLoaded = ModLoader.TryGetMod(FargosMutantName, out Mod frgoM);
            FargosMutantMod = frgoM;
        }

        public static void UnloadMods()
        {
            FablesIsLoaded = false;
            FablesMod = null;
            OrionIsLoaded = false;
            OrionMod = null;
            FargosSoulsIsLoaded = false;
            FargosSoulsMod = null;
            FargosMutantIsLoaded = false;
            FargosMutantMod = null;
        }
    }
}
