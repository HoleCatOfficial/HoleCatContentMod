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
using Terraria.DataStructures;

namespace DestroyerTest.Common
{
    public static class DTCrossMod
    {
        public const string CalamityName = "CalamityMod";
        public static bool CalamityIsLoaded;
        public static Mod CalamityMod;

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
            if (ModLoader.HasMod(CalamityName))
            {
                CalamityIsLoaded = ModLoader.TryGetMod(CalamityName, out Mod calamity);
                CalamityMod = calamity;
            }

            if (ModLoader.HasMod(FablesName))
            {
                FablesIsLoaded = ModLoader.TryGetMod(FablesName, out Mod fables);
                FablesMod = fables;
            }

            if (ModLoader.HasMod(OrionName))
            {
                OrionIsLoaded = ModLoader.TryGetMod(OrionName, out Mod orion);
                OrionMod = orion;
            }

            if (ModLoader.HasMod(FargosSoulsName))
            {
                FargosSoulsIsLoaded = ModLoader.TryGetMod(FargosSoulsName, out Mod frgoS);
                FargosSoulsMod = frgoS;
            }

            if (ModLoader.HasMod(FargosMutantName))
            { 
                FargosMutantIsLoaded = ModLoader.TryGetMod(FargosMutantName, out Mod frgoM);
                FargosMutantMod = frgoM;
            }
        }

        public static void UnloadMods()
        {
            CalamityIsLoaded = false;
            CalamityMod = null;
            FablesIsLoaded = false;
            FablesMod = null;
            OrionIsLoaded = false;
            OrionMod = null;
            FargosSoulsIsLoaded = false;
            FargosSoulsMod = null;
            FargosMutantIsLoaded = false;
            FargosMutantMod = null;
        }

        public static bool StealthStrike(this Projectile proj, Player Owner)
        {
            if (proj.TryGetGlobalProjectile<StealthStrikeGlobalProjectile>(out var Stealth))
            {
                return Stealth.StealthStrike;
            }
            return false;
        }

        public static bool StealthStrike(this Item item, Player player)
        {
            if (CalamityIsLoaded)
            {
                if (CalamityMod.Call("CanStealthStrike", player) is bool canStealth && canStealth && item.DamageType == ModContent.GetInstance<DTRogueClass>() || (CalamityMod.TryFind<DamageClass>("RogueDamageClass", out DamageClass rogueDamageClass) && item.DamageType == rogueDamageClass))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class StealthStrikeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool StealthStrike;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            if (DTCrossMod.CalamityIsLoaded)
            {
                if (DTCrossMod.CalamityMod.Call("CanStealthStrike", player) is bool canStealth && canStealth)
                {
                    StealthStrike = true;
                }

                if (projectile.DamageType == ModContent.GetInstance<DTRogueClass>() && DTCrossMod.CalamityMod.TryFind<DamageClass>("RogueDamageClass", out DamageClass rogueDamageClass))
                {
                    projectile.DamageType = rogueDamageClass;
                }
            }
        }
    }

    public class RogueCompatGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void SetDefaults(Item entity)
        {
                if (DTCrossMod.CalamityIsLoaded)
                {
                    /*
                    if (entity.DamageType == ModContent.GetInstance<DTRogueClass>() && DTCrossMod.CalamityMod.TryFind<DamageClass>("RogueDamageClass", out DamageClass rogueDamageClass))
                    {
                        entity.DamageType = rogueDamageClass;
                    }
                    */
                }
            
        }
    }
}