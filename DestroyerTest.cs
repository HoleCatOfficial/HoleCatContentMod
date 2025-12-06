using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Hjson;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.ModLoader;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Dusts;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI;
using DestroyerTest.Content.Magic;
using DestroyerTest.Common;
using ReLogic.Content;
using DestroyerTest.Content.Equips;
using Terraria.DataStructures;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Scepter;
using static Terraria.Graphics.FinalFractalHelper;
using OpusLib.Content.Helpers;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Tiles.RoseGarden;
using OpusLib;

namespace DestroyerTest
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class DestroyerTestMod : Mod
	{
        
        public static DTConfig Config;
        public static ModKeybind StarBlastKeybind { get; private set; }
        public static ModKeybind HeroHelmetKeybind { get; private set; }
        public static ModKeybind RiftTeleportKeybind { get; private set; }

        public static ModKeybind ManaBurstKeybind { get; private set; }
        public static ModKeybind TenebrisTeleportKeybind { get; private set; }

        public static ModKeybind DeadlyBlossomKeybind { get; private set; }
        public static ModKeybind OilTentacleKeybind { get; private set; }

        public static int RouletteTokenCurrencyId;

        public void AddChestLoot()
        {
            ChestLootSystem.RegisterChestLoot(
				ChestID.GoldLocked,
				ModContent.ItemType<EnchantedScepter>(),
				stack: 1,
				rarity: 0.3333333333f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.GoldLocked,
				ModContent.ItemType<FadedHood>(),
				stack: 1,
				rarity: 0.10f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.GoldLocked,
				ModContent.ItemType<FadedRobes>(),
				stack: 1,
				rarity: 0.10f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.SkyChest,
				ModContent.ItemType<TurbulenceScroll>(),
				stack: 1,
				rarity: 0.66666666666f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.SkyChest,
				ModContent.ItemType<StarScroll>(),
				stack: 1,
				rarity: 0.5f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.ShadowLocked,
				ModContent.ItemType<ShadowScepter>(),
				stack: 1,
				rarity: 0.80f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Water,
				ModContent.ItemType<CoralScepter>(),
				stack: 1,
				rarity: 0.66666666666f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<InsurgentCirclet>(),
				stack: 1,
				rarity: 0.15f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<InsurgentBodyArmor>(),
				stack: 1,
				rarity: 0.15f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<InsurgentFaulds>(),
				stack: 1,
				rarity: 0.15f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<RevenantMask>(),
				stack: 1,
				rarity: 0.15f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<ForgottenPendant>(),
				stack: 1,
				rarity: 0.5f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Frozen,
				ModContent.ItemType<FrigidScroll>(),
				stack: 1,
				rarity: 0.25f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Wooden,
				ModContent.ItemType<MageGlove>(),
				stack: 1,
				rarity: 0.35f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Wooden,
				ModContent.ItemType<RosyGlove>(),
				stack: 1,
				rarity: 0.35f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Wooden,
				ModContent.ItemType<MageGlove>(),
				stack: 1,
				rarity: 0.35f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<ScepterPolish>(),
				stack: 1,
				rarity: 0.35f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Sandstone,
				ModContent.ItemType<ThunderScepter>(),
				stack: 1,
				rarity: 0.25f
			);
            NightmareChestLoot();
        }

        public void NightmareChestLoot()
        {
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.CopperCoin,
				Min: 10,
				Max: 90,
				rarity: 0.7f
			);
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.SilverCoin,
				Min: 10,
				Max: 65,
				rarity: 0.7f
			);
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.GoldCoin,
				Min: 5,
				Max: 15,
				rarity: 0.7f
			);
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.PlatinumCoin,
				Min: 1,
				Max: 10,
				rarity: 0.7f
			);
            ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.MagicMirror,
				stack: 1,
				rarity: 0.10f
			);
            ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.UnholyArrow,
				Min: 6,
				Max: 22,
				rarity: 0.5f
			);
            ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				ItemID.WoodenArrow,
				Min: 10,
				Max: 40,
				rarity: 0.5f
			);
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<Tile_NightmareChest>(), 0),
				Opus.CommonPotion,
				rarity: 0.8f
			);
        }

        public override void Load()
        {
            Config = ModContent.GetInstance<DTConfig>();
            // Divider.
            StarBlastKeybind = KeybindLoader.RegisterKeybind(this, "Conclusion Star Blast", "P");
            // Divider.
            HeroHelmetKeybind = KeybindLoader.RegisterKeybind(this, "Hero Helmet Guard", "J");
            // Divider.
            RiftTeleportKeybind = KeybindLoader.RegisterKeybind(this, "Shadow Tome Teleport", "T");
            // Divider.
            // Divider.
            ManaBurstKeybind = KeybindLoader.RegisterKeybind(this, "Mana Burst", "C");
            // Divider.
            TenebrisTeleportKeybind = KeybindLoader.RegisterKeybind(this, "Tenebrous Clone Teleort", "L");
            // Divider.
            DeadlyBlossomKeybind = KeybindLoader.RegisterKeybind(this, "Deadly Blossom Spawn", "X");
            // Divider.
            OilTentacleKeybind = KeybindLoader.RegisterKeybind(this, "HoleCat Oil Tentacle", "OemTab");
            // Divider.
            RouletteTokenCurrencyId = CustomCurrencyManager.RegisterCurrency(new Content.Magic.RouletteToken(ModContent.ItemType<Content.Magic.RouletteTokenItem>(), 99L, "Mods.DestroyerTest.Content.Magic.RouletteToken"));
           
            var fractalProfiles = (Dictionary<int, FinalFractalProfile>)typeof(Terraria.Graphics.FinalFractalHelper).GetField("_fractalProfiles", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			fractalProfiles.Add(ModContent.ItemType<GargantuaZenith>(), new FinalFractalProfile(140f, new Color(255, 0, 0)));
			fractalProfiles.Add(ModContent.ItemType<Conclusion>(), new FinalFractalProfile(140f, ColorLib.StellarColor));

            AddChestLoot();
        }





        public override void Unload()
        {

            // Unregister the keybind
            StarBlastKeybind = null;
            HeroHelmetKeybind = null;
            RiftTeleportKeybind = null;
            ManaBurstKeybind = null;
            TenebrisTeleportKeybind = null;
            DeadlyBlossomKeybind = null;
            OilTentacleKeybind = null;

            var fractalProfiles = (Dictionary<int, FinalFractalProfile>)typeof(Terraria.Graphics.FinalFractalHelper).GetField("_fractalProfiles", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			fractalProfiles.Remove(ModContent.ItemType<GargantuaZenith>());
			fractalProfiles.Remove(ModContent.ItemType<Conclusion>());
        }

        public static bool EternityIsActive()
        {
            if (ModLoader.TryGetMod("FargowiltasSouls", out Mod frgo))
            {
                object result = frgo.Call("EternityMode");
                if (result is bool enabled)
                {
                    if (enabled)
                        return true;
                    else
                        return false;
                }
            }
            else
            {

            }
            return false;
        }

        public class WorldEnterPlayer : ModPlayer
        {
            public bool firstJoin = true;

            public override void OnEnterWorld()
            {
                Main.NewText("IMPORTANT INFO: The Wyvern Corpse Bossfight has been optimized the most it possibly could. It is unfortunately just a laggy boss.", Color.OrangeRed);
                firstJoin = true;
            }

            public override void UpdateDead()
            {
                firstJoin = false;
            }
        }
    }
}
