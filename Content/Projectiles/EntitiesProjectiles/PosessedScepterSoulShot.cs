using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using DestroyerTest.Common;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DestroyerTest.Content.Projectiles.EntitiesProjectiles
{
	public class PosessedScepterSoulShot : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.hostile = true;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
			trailOffset -= 0.01f;
			SpriteBatch spriteBatch = Main.spriteBatch;
			Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = ColorLib.PossessedScepterColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.SoulStreak.Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}
			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawGlowOnProj(Projectile, ColorLib.PossessedScepterColor, false, 0f);
			Opus.DrawTextureOnProj(DTAssetLib.PointGlow, Projectile, DTColorUtils.Pastel(ColorLib.PossessedScepterColor, 0.4f), false, 0f, 0.7f, 0.7f);
			Opus.DrawTextureOnProj(DTAssetLib.Star(2), Projectile, Color.White, false, 0f, S, S);
            Opus.ReturnToDefaultDrawing(spriteBatch);
			return false;
        }

		public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 500;
        private void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 0.5f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);
			}


			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);
        }

		public float S = 0f;
		public override void AI() 
		{
			CacheTrail();
			Projectile.rotation = Projectile.velocity.ToRotation();
			Lighting.AddLight(Projectile.Center, ColorLib.PossessedScepterColor.ToVector3() * 0.25f);
			S = Opus.Sine(0.75f, 1f);
		}

		public override void OnKill(int timeLeft) 
		{

		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SpiritDrift>(), 300);
        }
    }
}