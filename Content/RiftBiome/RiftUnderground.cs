using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome
{
	public class RiftUnderground : ModBiome
	{
		// Select all the scenery
		public override ModWaterStyle WaterStyle => ModContent.GetInstance<RiftWaterStyle>(); // Sets a water style for when inside this biome
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<RiftUndergroundBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Mushroom;

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
				else
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftUnderground");
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
            for (int t = 0; t < 5; t++)
            {
				Dust.NewDust(Main.screenPosition, Main.screenWidth, Main.screenHeight, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, -3));
            }
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


		// public override int BiomeTorchItemType => ModContent.ItemType<ExampleTorch>();
		// public override int BiomeCampfireItemType => ModContent.ItemType<ExampleCampfire>();

		// Populate the Bestiary Filter
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

		// Calculate when the biome is active.
		public override bool IsBiomeActive(Player player) {
			// First, we will use the exampleBlockCount from our added ModSystem for our first custom condition
			bool b1 = ModContent.GetInstance<RiftSurfaceTileCount>().RiftSurfaceBlockCount >= 15;

			// Second, we will limit this biome to the inner horizontal third of the map as our second custom condition
			//bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;

			// Finally, we will limit the height at which this biome can be active to above ground (ie sky and surface). Most (if not all) surface biomes will use this condition.
			bool b3 = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight;

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