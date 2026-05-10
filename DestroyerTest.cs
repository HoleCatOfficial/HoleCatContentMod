using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.MeleeWeapons.TwistedLineage;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Tiles.RoseGarden;
using Hjson;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Graphics.FinalFractalHelper;

namespace DestroyerTest
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class DestroyerTestMod : Mod
	{
        public override object Call(params object[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length == 0)
            {
                throw new ArgumentException("Arguments cannot be empty!");
            }

            if (args[0] is string content)
            {
                switch (content)
                {
                    case "RegisterPotion":
                    case "RegisterPotionFlowerPotion":
                    case "PotionFlowerPotion":
                    case "RegisterPotionForPotionFlower":
                        {
                            if (args[1] is string Name && args[2] is int ID && args[3] is int HealAmount)
                            {
                                PotionProfile P = new(Name, ID, HealAmount);
                                PotionFlowerPlayer.RegisterPotion(P);
                            }
                            break;
                        }

                    case "TenebrisEvilBiome":
                    case "TenebrisInCorruption":
                    case "TenebrisCorruption":
                        {
                            return DTFlags.TenebrisCanSpawnInWorldEvilBiome;
                        }
                 }
            }

            return false;
        }
        
        public static DTConfig Config;
        public static ModKeybind StarBlastKeybind { get; private set; }
        public static ModKeybind HeroHelmetKeybind { get; private set; }
        public static ModKeybind RiftTeleportKeybind { get; private set; }
		public static ModKeybind ArmorSetBonusHotKey { get; private set; }
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
				rarity: 0.70f
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
				rarity: 0.05f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<InsurgentBodyArmor>(),
				stack: 1,
				rarity: 0.05f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<InsurgentFaulds>(),
				stack: 1,
				rarity: 0.05f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<RevenantMask>(),
				stack: 1,
				rarity: 0.03f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<ForgottenPendant>(),
				stack: 1,
				rarity: 0.03f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Frozen,
				ModContent.ItemType<FrigidScroll>(),
				stack: 1,
				rarity: 0.10f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Wooden,
				ModContent.ItemType<MageGlove>(),
				stack: 1,
				rarity: 0.10f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Wooden,
				ModContent.ItemType<RosyGlove>(),
				stack: 1,
				rarity: 0.15f
			);
            ChestLootSystem.RegisterChestLoot(
				ChestID.Gold,
				ModContent.ItemType<ScepterPolish>(),
				stack: 1,
				rarity: 0.15f
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
			ArmorSetBonusHotKey = KeybindLoader.RegisterKeybind(this, "ArmorSetBonus", "Y");
            // Divider.
            ManaBurstKeybind = KeybindLoader.RegisterKeybind(this, "Mana Burst", "C");
            // Divider.
            TenebrisTeleportKeybind = KeybindLoader.RegisterKeybind(this, "Tenebrous Clone Teleort", "L");
            // Divider.
            DeadlyBlossomKeybind = KeybindLoader.RegisterKeybind(this, "Deadly Blossom Spawn", "X");
            // Divider.
            OilTentacleKeybind = KeybindLoader.RegisterKeybind(this, "HoleCat Oil Tentacle", "OemTab");
            // Divider.
            
           
            var fractalProfiles = (Dictionary<int, FinalFractalProfile>)typeof(Terraria.Graphics.FinalFractalHelper).GetField("_fractalProfiles", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            fractalProfiles.Add(ModContent.ItemType<Goliath>(), new FinalFractalProfile(110.30866f, Main.DiscoColor));
            fractalProfiles.Add(ModContent.ItemType<Gargantua>(), new FinalFractalProfile(172.53405f, Color.Red));

            fractalProfiles.Add(ModContent.ItemType<Constitution>(), new FinalFractalProfile(72.13876f, ColorLib.StellarFireGradientLooping()));
            fractalProfiles.Add(ModContent.ItemType<Committment>(), new FinalFractalProfile(157.13688f, Main.DiscoColor));

            fractalProfiles.Add(ModContent.ItemType<RiftHypersabre>(), new FinalFractalProfile(161.24515f, ColorLib.Rift));
            fractalProfiles.Add(ModContent.ItemType<SoulEdge>(), new FinalFractalProfile(67.88225f, Color.SkyBlue));

            fractalProfiles.Add(ModContent.ItemType<DarkFireSword>(), new FinalFractalProfile(135.7645f, Color.DarkMagenta));
            fractalProfiles.Add(ModContent.ItemType<TwilightInferno>(), new FinalFractalProfile(169.70563f, Color.DarkMagenta));
            fractalProfiles.Add(ModContent.ItemType<Exasperation>(), new FinalFractalProfile(164.04877f, Color.DarkMagenta));

            AddChestLoot();

			DTCrossMod.LoadMods();
        }





        public override void Unload()
        {

            // Unregister the keybind
            StarBlastKeybind = null;
            HeroHelmetKeybind = null;
            RiftTeleportKeybind = null;
			ArmorSetBonusHotKey = null;
            ManaBurstKeybind = null;
            TenebrisTeleportKeybind = null;
            DeadlyBlossomKeybind = null;
            OilTentacleKeybind = null;

            var fractalProfiles = (Dictionary<int, FinalFractalProfile>)typeof(Terraria.Graphics.FinalFractalHelper).GetField("_fractalProfiles", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            fractalProfiles.Remove(ModContent.ItemType<Goliath>());
            fractalProfiles.Remove(ModContent.ItemType<Gargantua>());

            fractalProfiles.Remove(ModContent.ItemType<Constitution>());
            fractalProfiles.Remove(ModContent.ItemType<Committment>());

            fractalProfiles.Remove(ModContent.ItemType<RiftHypersabre>());
            fractalProfiles.Remove(ModContent.ItemType<SoulEdge>());
			
            fractalProfiles.Remove(ModContent.ItemType<DarkFireSword>());
            fractalProfiles.Remove(ModContent.ItemType<TwilightInferno>());
            fractalProfiles.Remove(ModContent.ItemType<Exasperation>());

            DTCrossMod.UnloadMods();
        }

        private static bool Eternity()
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

		public static bool EternityIsActive => Eternity();

		public static bool MasochistIsActive => EternityIsActive && Main.masterMode;

        public class WorldEnterPlayer : ModPlayer
        {
            public bool firstJoin = true;

            public override void OnEnterWorld()
            {
				Main.NewText($"You are running Talid v{Mod.Version.ToString()}", ColorLib.Rift);
                firstJoin = true;
            }

            public override void UpdateDead()
            {
                firstJoin = false;
            }
        }
    }
}
