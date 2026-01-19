using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using OpusLib;
using ReLogic.Content;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class GhoulProjectile : ModProjectile
	{
		public override void SetStaticDefaults() 
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.penetrate = 2;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}


		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;

			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();
			Asset<Texture2D> strip = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/GhoulStreak");

			Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 16;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 16;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, 0);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = strip.Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}


			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 200;
		private void CacheTrail()
		{
			Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 1f; // how closely to sample. tweak this!

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

		public override void AI() 
		{
			CacheTrail();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (Projectile.Center.Distance(Main.MouseWorld) < 20f)
			{
				Projectile.velocity *= 0.8f;
			}
			else
			{
				float length = Projectile.velocity.Length();
				float targetAngle = Projectile.AngleTo(Main.MouseWorld);
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(10)).ToRotationVector2() * length;
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
				Projectile.velocity *= 1.1f;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 600);
        }

    }
	
}