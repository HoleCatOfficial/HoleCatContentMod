using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources.Cloths;
using DetroyerTest.Content.RiftBiome;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Events;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using DestroyerTest.Content.Projectiles;

namespace DestroyerTest.Content.RiftBiome
{

	public class RiftDesert : ModBiome
	{
		public override ModWaterStyle WaterStyle => ModContent.GetInstance<RiftWaterStyle>(); // Sets a water style for when inside this biome
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<RiftDesertBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

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
				if (Sandstorm.Happening)
				{
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftSandstorm");
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
					return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftDesert");
				}

			}
		}

		public override void OnInBiome(Player player)
		{
			if (!player.HasBuff<StoneLungs>() && !player.HasBuff<AirSeal>())
			{
				player.AddBuff(BuffID.Suffocation, 360);
			}
			Rectangle ScreenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
			if (!Main.dedServ && !optcfg.DisableExcessDusts)
			{
				for (int t = 0; t < 5; t++)
				{
					Dust.NewDust(Main.screenPosition, Main.screenWidth, Main.screenHeight, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, -3));
				}
			}
			
			SetBiomeProperties(player);
			if (Sandstorm.Happening)
			{
				SandStormFX(ScreenRect, 30, Main.rand.NextFloat(-1.5f, -3));
				Main.eclipseLight = 1f;
				player.AddBuff(BuffID.Obstructed, 60);
				if (Main.rand.NextBool(100))
				{
					player.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
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
        }

		public void SandStormFX(Rectangle area, float speedX, float speedY)
		{
			for (int t = 0; t < 45; t++)
			{
				Dust.NewDust(area.TopLeft(), area.Left, area.Height, ModContent.DustType<RiftDust>(), speedX, speedY);
				Dust.NewDust(area.TopLeft(), area.Left, area.Height, DustID.Wraith, speedX, speedY);
			}
		}

		public override string BestiaryIcon => "DestroyerTest/Assets/Textures/RiftIcon";
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => BackgroundPath;

		public override bool IsBiomeActive(Player player)
		{
			
			bool b1 = ModContent.GetInstance<RiftDesertTileCount>().RiftDesertBlockCount >= 40;
			bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight;
			return b1 && b3;
		}

		public override SceneEffectPriority Priority
		{
			get
			{
				if (Main.eclipse || Main.bloodMoon || Main.snowMoon || Main.pumpkinMoon)
				{
					return SceneEffectPriority.BossMedium;
				}
				if (Main.snowMoon || Main.pumpkinMoon)
				{
					return SceneEffectPriority.BossHigh; 
				}
				return SceneEffectPriority.BiomeMedium;
			}
		}
	}
	
	
}