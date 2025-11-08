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

namespace DestroyerTest.Content.RiftBiome
{
	// Shows setting up two basic biomes. For a more complicated example, please request.
	public class RiftDesert : ModBiome
	{
		// Select all the scenery
		public override ModWaterStyle WaterStyle => ModContent.GetInstance<RiftWaterStyle>(); // Sets a water style for when inside this biome
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<RiftDesertBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
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
			ModifyMusic(Music, Priority);
			if (!player.HasBuff<StoneLungs>() && !player.HasBuff<AirSeal>())
			{
				player.AddBuff(BuffID.Suffocation, 360); // Apply the suffocation buff if all conditions are met
			}
			Rectangle ScreenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			for (int t = 0; t < 5; t++)
			{
				Dust.NewDust(Main.screenPosition, Main.screenWidth, Main.screenHeight, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, -3));
			}

			if (Sandstorm.Happening)
			{
				SandStormFX(ScreenRect, 30, Main.rand.NextFloat(-1.5f, -3));
				if (Main.rand.NextBool(100))
				{
					player.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
				}
			}
		}

		public void SandStormFX(Rectangle area, float speedX, float speedY)
		{
			for (int t = 0; t < 45; t++)
			{
				Dust.NewDust(area.TopLeft(), area.Left, area.Height, ModContent.DustType<RiftDust>(), speedX, speedY);
				Dust.NewDust(area.TopLeft(), area.Left, area.Height, DustID.Wraith, speedX, speedY);
			}

			foreach (Projectile proj in Main.projectile)
            {
                if (!proj.active && proj.type == ModContent.ProjectileType<RiftSandstormBackgroundProj>())
                {
					Projectile.NewProjectile(Entity.GetSource_None(), Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<RiftSandstormBackgroundProj>(), 0, 0, -1);
                }
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
		public override bool IsBiomeActive(Player player)
		{
			// First, we will use the exampleBlockCount from our added ModSystem for our first custom condition
			bool b1 = ModContent.GetInstance<RiftDesertTileCount>().RiftDesertBlockCount >= 15;

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
	
	public class RiftSandstormBackgroundProj : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Extras/FadeLine";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 248000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
		{
			Player player = Main.LocalPlayer;

			if (player.InModBiome<RiftDesert>() && Sandstorm.Happening)
			{
				Projectile.active = true;
			}
			else
            {
				Projectile.active = false;
            }
			Projectile.Center = player.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D BGTex = DTAssetLib.TilableNoise(5).Value;
            Texture2D BGTex2 = DTAssetLib.TilableNoise(5).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            float t = (float)Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f;
            Color drawColor = Color.Lerp(Color.Black, ColorLib.Rift * 0.5f, t);

            if (!optcfg.OptimizeGame)
            {
                

                float time = (float)Main.GameUpdateCount / 60f;

                // --- Layer 1 scroll parameters ---
                float scrollSpeedX1 = -600f;
                float scrollSpeedY1 = 30f;

                float scrollOffsetX1 = (time * scrollSpeedX1) % BGTex.Width;
                float scrollOffsetY1 = (time * scrollSpeedY1) % BGTex.Height;

                int screenW = Main.screenWidth;
                int screenH = Main.screenHeight;

                // --- draw one tile beyond each edge ---
                float startX = -BGTex.Width;
                float startY = -BGTex.Height;
                float endX = screenW + BGTex.Width;
                float endY = screenH + BGTex.Height;

				// --- Draw first layer ---
				for (float x = -scrollOffsetX1 + startX; x < endX; x += BGTex.Width)
				{
					for (float y = -scrollOffsetY1 + startY; y < endY; y += BGTex.Height)
					{
						spriteBatch.Draw(BGTex, new Vector2(x, y), null, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
					}
				}

				Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                float scrollSpeedX2 = 600f;
                float scrollSpeedY2 = -60f; // opposite direction for contrast

                float scrollOffsetX2 = (time * scrollSpeedX2) % BGTex2.Width;
                float scrollOffsetY2 = (time * scrollSpeedY2) % BGTex2.Height;

                Color drawColor2 = drawColor * 0.8f; // slightly dimmer to layer properly

                // --- Draw second layer ---
                for (float x = -scrollOffsetX2 + startX; x < endX; x += BGTex2.Width)
                {
                    for (float y = -scrollOffsetY2 + startY; y < endY; y += BGTex2.Height)
                    {
                        spriteBatch.Draw(BGTex2, new Vector2(x, y), null, drawColor2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
            return false;
        }



    }
}