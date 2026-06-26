using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.MeleeWeapons.TwistedLineage;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Tiles.RoseGarden;
using Hjson;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
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
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
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

            ChestLootSystem.RegisterChestLoot(
                ChestID.Ivy,
                ItemID.JungleRose,
                stack: 1,
                rarity: 0.1f
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

        //Fables strategy
        private static readonly Type UIModItemType = typeof(ModItem).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
        private static readonly MethodInfo InitializeModItemUIMethod = UIModItemType?.GetMethod("OnInitialize", BindingFlags.Instance | BindingFlags.Public);
        private static readonly PropertyInfo ModNameProperty = UIModItemType?.GetProperty("ModName", BindingFlags.Instance | BindingFlags.Public);
        private static readonly FieldInfo ModIconField = UIModItemType?.GetField("_modIcon", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo ModNameElement = UIModItemType?.GetField("_modName", BindingFlags.Instance | BindingFlags.NonPublic);
        public delegate void orig_OnInitialize(UIElement self);
        public static Hook CreateIconAnimation;

        public void AnimatedModIcon(orig_OnInitialize orig, UIElement self)
        {
            orig(self);

            var element = (UIElement)self;


            if (!element.GetType().IsAssignableTo(UIModItemType))
                return;

            object potentialModName = ModNameProperty.GetValue(element);
            if (potentialModName == null || potentialModName is not string modName || modName != "DestroyerTest")
                return;

            object potentiallyTheIcon = ModIconField.GetValue(element);
            if (potentiallyTheIcon is UIImage modIconImage)
            {
                AnimatedIcon addedDrawLogic = new AnimatedIcon((UIText)ModNameElement.GetValue(element));
                modIconImage.Append(addedDrawLogic);
                modIconImage.Color = Color.Transparent;
            }
        }

        public override void Load()
        {

            if (InitializeModItemUIMethod != null && ModIconField != null && ModNameProperty != null)
            {
                CreateIconAnimation = new Hook(InitializeModItemUIMethod, AnimatedModIcon);
            }

            Config = ModContent.GetInstance<DTConfig>();

            // Divider.
            RiftTeleportKeybind = KeybindLoader.RegisterKeybind(this, "Shadow Tome Teleport", "T");
            // Divider.
			ArmorSetBonusHotKey = KeybindLoader.RegisterKeybind(this, "ArmorSetBonus", "Y");
            // Divider.
            TenebrisTeleportKeybind = KeybindLoader.RegisterKeybind(this, "Tenebrous Clone Teleort", "L");
            // Divider.
            DeadlyBlossomKeybind = KeybindLoader.RegisterKeybind(this, "Deadly Blossom Spawn", "X");

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
            if (InitializeModItemUIMethod == null || ModIconField == null || CreateIconAnimation == null)
                return;

            CreateIconAnimation.Undo();
            CreateIconAnimation.Dispose();
            CreateIconAnimation = null;


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

        //I hate example mod organization. Why do I need all this bullshit in another class?
        internal enum MessageType : byte
        {
            HeliciteManaSync
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                // This message syncs ExampleStatIncreasePlayer.exampleLifeFruits and ExampleStatIncreasePlayer.exampleManaCrystals
                case MessageType.HeliciteManaSync:
                    byte playerNumber = reader.ReadByte();
                    HeliciteManaPlayer examplePlayer = Main.player[playerNumber].GetModPlayer<HeliciteManaPlayer>();
                    examplePlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        examplePlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
       
                default:
                    Logger.WarnFormat("DestroyerTest: Unknown Message type: {0}", msgType);
                    break;
            }
        }

    }

    public class AnimatedIcon : UIElement
    {
        public UIText ModName;

        public AnimatedIcon(UIText nameUI)
        {
            Width.Set(80, 0f);
            Height.Set(80, 0f);

            ModName = nameUI;
        }

        public static Asset<Texture2D> Texture => ModContent.Request<Texture2D>("DestroyerTest/icon-sheet");

        int InternalTimer = 0;
        int frameCount = 11;
        int currentFrame = 0;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 centerOfIcon = dimensions.Center();

            InternalTimer++;

            if (InternalTimer % 10 == 0)
            {
                currentFrame++;
                if (currentFrame >= frameCount)
                {
                    currentFrame = 0;
                }
            }

            Rectangle Frame = new Rectangle(0, currentFrame * 80, 80, 80);

            spriteBatch.Draw(Texture.Value, centerOfIcon, Frame, Color.White, 0f, new Vector2(40, 40), 1f, SpriteEffects.None, 0f);
        
            if (Main.rand.NextBool(3))
            {
                Spark spark = new();
                spark.PrepareSpark(centerOfIcon, new Vector2(3, 0), 0f, Color.Red, 1f, false, 70, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark);
            }
        }
    }
}
