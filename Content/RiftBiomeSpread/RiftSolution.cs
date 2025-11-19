
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.RiftBiome.RiftTundraResources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiomeSpread
{
	public class RiftSolution : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
			ItemID.Sets.SortingPriorityTerraforming[Type] = 101; // One past dirt solution
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
		}

		public override void SetDefaults() {
			Item.DefaultToSolution(ModContent.ProjectileType<RiftSolutionProjectile>());
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Solutions;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            var RW = Language.GetText("Mods.DestroyerTest.Items.RiftSolution.RiftWarning");
			var riftTip = new TooltipLine(Mod, "Rift Warning", RW.Value) { OverrideColor = ColorLib.Rift };
			tooltips.Add(riftTip);
        }


		public override void AddRecipes() {
			CreateRecipe(3)
				.AddIngredient<Living_Shadow>(6)
                .AddIngredient(ItemID.BottledWater, 1)
				.Register();
		}
	}

	public class RiftSolutionProjectile : ModProjectile
	{
		public static int ConversionType;

		public ref float Progress => ref Projectile.ai[0];
		// Solutions shot by the terraformer get an increase in conversion area size, indicated by the second AI parameter being set to 1
		public bool ShotFromTerraformer => Projectile.ai[1] == 1f;

		public override void SetStaticDefaults()
		{
			// Cache the conversion type here instead of repeately fetching it every frame
			ConversionType = ModContent.GetInstance<RiftConversion>().Type;
		}

		public override void SetDefaults()
		{
			// This method quickly sets the projectile properties to match other sprays.
			Projectile.DefaultToSpray();
			Projectile.aiStyle = 0; // Here we set aiStyle back to 0 because we have custom AI code
			Projectile.alpha = 255;
		}

		public override bool? CanDamage() => false;

		public override void AI()
		{

			if (Projectile.timeLeft > 133)
				Projectile.timeLeft = 133;

			if (Projectile.owner == Main.myPlayer)
			{
				int size = ShotFromTerraformer ? 3 : 2;
				Point tileCenter = Projectile.Center.ToTileCoordinates();
				WorldGen.Convert(tileCenter.X, tileCenter.Y, ConversionType, size);
			}

			int spawnDustTreshold = 7;
			if (ShotFromTerraformer)
				spawnDustTreshold = 3;

			if (Progress > (float)spawnDustTreshold)
			{
				float dustScale = 1f;
				int dustType = ModContent.DustType<RiftDust>();

				if (Progress == spawnDustTreshold + 1)
					dustScale = 0.2f;
				else if (Progress == spawnDustTreshold + 2)
					dustScale = 0.4f;
				else if (Progress == spawnDustTreshold + 3)
					dustScale = 0.6f;
				else if (Progress == spawnDustTreshold + 4)
					dustScale = 0.8f;

				int dustArea = 0;
				if (ShotFromTerraformer)
				{
					dustScale *= 1.2f;
					dustArea = (int)(12f * dustScale);
				}

				Dust sprayDust = Dust.NewDustDirect(new Vector2(Projectile.position.X - dustArea, Projectile.position.Y - dustArea), Projectile.width + dustArea * 2, Projectile.height + dustArea * 2, dustType, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100);
				sprayDust.noGravity = true;
				sprayDust.scale *= 1.75f * dustScale;
			}

			Progress++;
			Projectile.rotation += 0.3f * Projectile.direction;
		}
	}

	public class RiftConversion : ModBiomeConversion
	{
		public static int WallType;
		public static int UnsafeWallType;
		public static int GrassType;
		public static int DirtType;
		public static int StoneType;
		public static int SandType;
		public static int HardenedSandType;
		public static int SandstoneType;
		public static int ClayType;
		public static int SnowType;
		public static int IceType;
		public static int[] Grasses = new int[]
		{
			TileID.Grass,
			TileID.GolfGrass,
			TileID.GolfGrassHallowed,
			TileID.CorruptGrass,
			TileID.CrimsonGrass,
			TileID.HallowedGrass,
		};

		public static int[] Mosses = new int[]
		{
			TileID.RedMoss,
			TileID.BlueMoss,
			TileID.LavaMoss,
			TileID.LavaMossBlock,
			TileID.LongMoss,
			TileID.ArgonMoss,
			TileID.ArgonMossBlock,
			TileID.BrownMoss,
			TileID.GreenMoss,
			TileID.XenonMoss,
			TileID.XenonMossBlock,
			TileID.PurpleMoss,
			TileID.VioletMoss,
			TileID.VioletMossBlock,
			TileID.KryptonMoss,
			TileID.KryptonMossBlock,
			TileID.RainbowMoss,
			TileID.RainbowMossBlock
		};

		public override void PostSetupContent() {

			// Cache the conversion types.
			WallType = ModContent.WallType<Wall_RiftWall>();
			UnsafeWallType = ModContent.WallType<Wall_DangerousRiftWall>();
			GrassType = ModContent.TileType<Tile_RiftDirt>();
			DirtType = ModContent.TileType<Tile_RiftDirt>();
			StoneType = ModContent.TileType<Tile_RiftStone>();
			SandType = ModContent.TileType<Tile_RiftSilt>();
			HardenedSandType = ModContent.TileType<Tile_HardenedRiftSilt>();
			SandstoneType = ModContent.TileType<Tile_RiftSiltStone>();
			ClayType = ModContent.TileType<Tile_RiftClay>();
			SnowType = ModContent.TileType<Tile_RiftSnow>();
			IceType = ModContent.TileType<Tile_RiftIce>();

			// Normally we'd just use WallLoader.RegisterSimpleConversion on the basic wall types and rely on the fallback system
			// but we want to convert safe walls to safe example walls and unsafe to unsafe, where vanilla convers safe walls to unsafe walls on all conversions
			for (int i = 0; i < WallLoader.WallCount; i++) {
				if (WallID.Sets.Conversion.Dirt[i] ||
					WallID.Sets.Conversion.Grass[i] ||
					WallID.Sets.Conversion.Stone[i] ||
					WallID.Sets.Conversion.Sandstone[i] ||
					WallID.Sets.Conversion.HardenedSand[i] ||
					WallID.Sets.Conversion.Ice[i] ||
					WallID.Sets.Conversion.NewWall1[i] || // NewWalls are the underground wall variants 
					WallID.Sets.Conversion.NewWall2[i] ||
					WallID.Sets.Conversion.NewWall3[i] ||
					WallID.Sets.Conversion.NewWall4[i])
					WallLoader.RegisterConversion(i, Type, ConvertWalls);
			}
			WallLoader.RegisterConversionFallback(WallType, WallID.Dirt, Type);
			WallLoader.RegisterConversionFallback(UnsafeWallType, WallID.DirtUnsafe, Type);

			// This registers a conversion from Sand to ExampleSand, as well as a fallback from ExampleSand to Sand, so other solutions can convert ExampleSand (eg to Crimsand)
			TileLoader.RegisterSimpleConversion(TileID.Sand, Type, SandType);
			TileLoader.RegisterSimpleConversion(TileID.Dirt, Type, DirtType);
			foreach (int g in Grasses)
			{
				TileLoader.RegisterSimpleConversion(g, Type, DirtType);
			}
			foreach (int m in Mosses)
			{
				TileLoader.RegisterSimpleConversion(m, Type, StoneType);
			}
			TileLoader.RegisterSimpleConversion(TileID.Stone, Type, StoneType);
			TileLoader.RegisterSimpleConversion(TileID.HardenedSand, Type, HardenedSandType);
			TileLoader.RegisterSimpleConversion(TileID.Sandstone, Type, SandstoneType);
			TileLoader.RegisterSimpleConversion(TileID.ClayBlock, Type, ClayType);
			TileLoader.RegisterSimpleConversion(TileID.SnowBlock, Type, SnowType);
			TileLoader.RegisterSimpleConversion(TileID.IceBlock, Type, IceType);

		}

		public bool ConvertWalls(int i, int j, int type, int conversionType) {

			// Turn all walls into example walls or unsafe example walls, depending on if the original wall was safe or not (Main.wallHouse is what determines that)
			int wallType = Main.wallHouse[type] ? WallType : UnsafeWallType;
			WorldGen.ConvertWall(i, j, wallType);
			return false;
		}
	}
}