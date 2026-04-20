using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame
{
	public class CursedFlameVortex : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;
		private Player HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults() {
			
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.alpha = 255;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 0.1f;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.CursedFlames;
			trailOffset += 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
                List<ColoredVertex> ve2 = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float u = i / (float)(TrailPositions.Count - 1);
					float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

					float width = 32f * widthFactor;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
					Vector2 offset  = perp * width;
					Vector2 offset2 = -perp * width;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, u, lightColor, trailOffset);
				}

                for (int i = TrailPositions2.Count - 1; i > 0; i--)
				{
					float u = i / (float)(TrailPositions.Count - 1);
					float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

					float width = 32f * widthFactor;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
					Vector2 offset  = perp * width;
					Vector2 offset2 = -perp * width;

					DTUtils.AddStrips(ve2, TrailPositions2, i, offset, offset2, u, lightColor, trailOffset);
				}

				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(9).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    gd.Textures[0] = DTAssetLib.Streak(10).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();

        public List<Vector2> TrailPositions2 = new();
		public List<float> TrailRotations2 = new();
		private const int TrailLength = 40;
		public override void AI()
		{
			Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 8f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, Projectile.rotation);
                    TrailPositions2.Insert(0, pos);
					TrailRotations2.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);
                TrailPositions2.Insert(0, newPos);
				TrailRotations2.Insert(0, Projectile.rotation);
			}

			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

            while (TrailPositions2.Count > TrailLength)
				TrailPositions2.RemoveAt(TrailPositions2.Count - 1);
			while (TrailRotations2.Count > TrailLength)
				TrailRotations2.RemoveAt(TrailRotations2.Count - 1);
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (var trail in new[] { TrailPositions, TrailPositions2 })
            {
                for (int i = 1; i < trail.Count; i++)
                {
                    Vector2 point1 = trail[i - 1];
                    Vector2 point2 = trail[i];
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), point1, point2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}