using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DetroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;

namespace DestroyerTest.Content.RiftBiome
{
	// Shows setting up two basic biomes. For a more complicated example, please request.
	public class RiftSurface : ModBiome
	{
		// Select all the scenery
		public override ModWaterStyle WaterStyle => ModContent.GetInstance<RiftWaterStyle>(); // Sets a water style for when inside this biome
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<RiftSurfaceBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Mushroom;
		// Select Music

		public override int Music
		{
			get
			{
				if (Main.eclipse == true)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftEvent");
				}
				if (Main.bloodMoon == true)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftEvent");
				}
				if (Main.snowMoon == true)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftEvent");
				}
				if (Main.pumpkinMoon == true)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftEvent");
				}
				if (Main.getGoodWorld == true)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftEvent");
				}
				if (Main.maxRaining < 0.5f && Main.maxRaining > 0f)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftRain");
				}
				if (Main.maxRaining >= 0.5f)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftThunderstorm");
				}
				else
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftV2");
				}

			}
		}

		public override void OnInBiome(Player player)
		{
			ModifyMusic(Music, Priority);
			if (!player.HasBuff<StoneLungs>() && !player.HasBuff<AirSeal>())
			{
				player.AddBuff(BuffID.Suffocation, 360); // Apply the suffocation buff if all conditions are met
			}

			SetBiomeProperties(player);
			Rectangle ScreenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
			if (!Main.dedServ && !optcfg.DisableExcessDusts)
			{
				for (int t = 0; t < 5; t++)
				{
					Dust.NewDust(Main.screenPosition, Main.screenWidth, Main.screenHeight, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, -3));
				}
			}
		}

		public void SetBiomeProperties(Player player)
        {
			player.ZoneDesert = false;
			player.ZonePurity = false;
			player.ZoneCorrupt = false;
			player.ZoneCrimson = false;
			player.ZoneDungeon = false;
			player.ZoneHallow = false;
			player.ZoneJungle = false;
			player.ZoneSnow = false;
			player.ZoneGlowshroom = false;
			player.ZoneBeach = false;
			player.ZoneGranite = false;
			player.ZoneMarble = false;
			player.ZoneHive = false;
			player.ZoneLihzhardTemple = false;
			player.ZoneShimmer = false;

			if (Main.IsItRaining && !Main.dedServ)
            {
				List<int> GraveyardClouds = new List<int>
                {
                  GoreID.AmbientFloorCloud1,
				  GoreID.AmbientFloorCloud2,
				  GoreID.AmbientFloorCloud3,
				  GoreID.AmbientFloorCloud4
                };
                Vector2 OpenTileCoords = GetAirTileOnGround();
				if (OpenTileCoords != Vector2.Zero)
                {
                    Gore.NewGore(Entity.GetSource_None(), OpenTileCoords, new Vector2(Main.rand.NextFloat(-2, 2), 0), GraveyardClouds[Main.rand.Next(GraveyardClouds.Count)], 3);
                }
            }
        }

		public Vector2 GetAirTileOnGround()
		{
			int playerTileX = (int)(Main.LocalPlayer.Center.X / 16);
			int playerTileY = (int)(Main.LocalPlayer.Center.Y / 16);
			int searchRadius = 200;

			for (int x = playerTileX - searchRadius; x < playerTileX + searchRadius; x++)
			{
				for (int y = playerTileY - searchRadius; y < playerTileY + searchRadius; y++)
				{
					if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
						continue;

					Tile tile = Main.tile[x, y];

					// Must be air
					if (tile.HasTile)
						continue;

					// Must have solid tile directly below
					if (y + 1 >= Main.maxTilesY || !Main.tile[x, y + 1].HasTile)
						continue;

					// Must have 30 tiles of air ABOVE
					bool openAbove = true;
					for (int checkY = y - 1; checkY > y - 30; checkY--)
					{
						if (checkY < 0)
							break;

						if (Main.tile[x, checkY].HasTile)
						{
							openAbove = false;
							break;
						}
					}

					if (!openAbove)
						continue;

					// Found a valid open-air-above tile
					return new Vector2((x + 0.5f) * 16f, (y + 0.5f) * 16f);
				}
			}

			return Vector2.Zero;
		}



		public void ModifyMusic(int music, SceneEffectPriority priority)
		{
			if (Main.snowMoon || Main.pumpkinMoon)
			{
				FieldInfo eventMusicField = typeof(Main).GetField("curMusic", BindingFlags.NonPublic | BindingFlags.Static);
				eventMusicField?.SetValue(null, MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftEvent"));

				priority = SceneEffectPriority.BossHigh;
			}
		}

		public static Condition InRift = new Condition("InRift", () => Main.LocalPlayer.InModBiome<RiftSurface>() || Main.LocalPlayer.InModBiome<RiftUnderground>() || Main.LocalPlayer.InModBiome<RiftDesert>() || Main.LocalPlayer.InModBiome<RiftTundra>());

		// public override int BiomeTorchItemType => ModContent.ItemType<ExampleTorch>();
		// public override int BiomeCampfireItemType => ModContent.ItemType<ExampleCampfire>();

		// Populate the Bestiary Filter
		public override string BestiaryIcon => "DestroyerTest/Assets/Textures/RiftIcon";
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

		// Calculate when the biome is active.
		public override bool IsBiomeActive(Player player) {
			// First, we will use the exampleBlockCount from our added ModSystem for our first custom condition
			bool b1 = ModContent.GetInstance<RiftSurfaceTileCount>().RiftSurfaceBlockCount >= 40;

			// Second, we will limit this biome to the inner horizontal third of the map as our second custom condition
			//bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;

			// Finally, we will limit the height at which this biome can be active to above ground (ie sky and surface). Most (if not all) surface biomes will use this condition.
			bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight;

			return b1 && b3;
		}

		// Declare biome priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
		public override SceneEffectPriority Priority
		{
			get
			{
				if (Main.eclipse || Main.bloodMoon || Main.snowMoon || Main.pumpkinMoon)
				{
					return SceneEffectPriority.BossMedium; // BossHigh is stronger than Event priority
				}
				if (Main.snowMoon || Main.pumpkinMoon)
				{
					return SceneEffectPriority.BossHigh; // BossHigh is stronger than Event priority
				}
				return SceneEffectPriority.BiomeMedium; // Normal priority otherwise
			}
		}
	}
}