using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using DestroyerTest.Content.Equips;

namespace DestroyerTest.Content.Projectiles
{
	public class TenebrisStarHostile : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;

		private Player PLRTarget
		{
			get => Projectile.ai[1] == 0 ? null : Main.player[(int)Projectile.ai[1] - 1];
			set
			{
				Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.TenebrisGradient;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
                {
                    float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
                    Color b = ColorLib.TenebrisGradient * t;

                    Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                    Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

                    ve.Add(new ColoredVertex(
                        TrailPositions[i] - Main.screenPosition + offset,
                        new Vector3(t, 1, 1),
                        b));

                    ve.Add(new ColoredVertex(
                        TrailPositions[i] - Main.screenPosition + offset2,
                        new Vector3(t, 0, 1),
                        b));
                }


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(1).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}

            /*
			for (int i = 0; i < TrailPositions.Count - 1; i++)
			{
				Vector2 start = TrailPositions[i] - Main.screenPosition;
				Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
				Vector2 diff = end - start;

				float length = diff.Length();
				if (length < 0.5f)
					continue;

				float rotation = diff.ToRotation();
				float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
				float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);

				// --- Tenebris gradient with offset ---
				float time = (Main.GlobalTimeWrappedHourly + i * 0.05f) % 3f;

				Color tenebrisColor;
				if (time < 1f)
					tenebrisColor = Color.Lerp(ColorLib.TenebrisBeige, ColorLib.TenebrisMagenta, time);
				else if (time < 2f)
					tenebrisColor = Color.Lerp(ColorLib.TenebrisMagenta, ColorLib.TenebrisBlue, time - 1f);
				else
					tenebrisColor = Color.Lerp(ColorLib.TenebrisBlue, ColorLib.TenebrisBeige, time - 2f);

				tenebrisColor *= alpha;

				Main.spriteBatch.Draw(
					DTAssetLib.Square.Value,
					start,
					null,
					tenebrisColor,
					rotation,
					new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
					new Vector2(length, width),
					SpriteEffects.None,
					0f
				);
			}
            */

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Opus.DrawTextureOnProj(DTAssetLib.Star(1), Projectile, Color.White, true, 0f, 0.35f, 0.35f);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10;
        }


		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
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

			DelayTimer++;
			Projectile.rotation += Projectile.direction * 0.07f;

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

			if (DelayTimer < 20 || DelayTimer > 180)
			{
				return;
			}

			float maxDetectRadius = 2800f;

            if (PLRTarget == null)
            {
                PLRTarget = FindClosestPlayer(maxDetectRadius);
            }


            if (PLRTarget != null && !IsValidPlayer(PLRTarget))
            {
                PLRTarget = null;
            }

            if (PLRTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(PLRTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * length;
        
		}

		public Player FindClosestPlayer(float maxDetectDistance)
		{
			Player closestPlayer = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.player)
			{
				if (IsValidPlayer(target))
				{
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestPlayer = target;
					}
				}
			}

			return closestPlayer;
		}

		public bool IsValidPlayer(Player target)
		{
			return target.active == true && target.statLife > 1;
		}

		

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 30 * 60);
		}

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }

    }

    public class TenebrisStarHostile_NoHoming : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;


		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.TenebrisGradient;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
                {
                    float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
                    Color b = ColorLib.TenebrisGradient * t;

                    Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                    Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

                    ve.Add(new ColoredVertex(
                        TrailPositions[i] - Main.screenPosition + offset,
                        new Vector3(t, 1, 1),
                        b));

                    ve.Add(new ColoredVertex(
                        TrailPositions[i] - Main.screenPosition + offset2,
                        new Vector3(t, 0, 1),
                        b));
                }


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(1).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}

            /*
			for (int i = 0; i < TrailPositions.Count - 1; i++)
			{
				Vector2 start = TrailPositions[i] - Main.screenPosition;
				Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
				Vector2 diff = end - start;

				float length = diff.Length();
				if (length < 0.5f)
					continue;

				float rotation = diff.ToRotation();
				float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
				float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);

				// --- Tenebris gradient with offset ---
				float time = (Main.GlobalTimeWrappedHourly + i * 0.05f) % 3f;

				Color tenebrisColor;
				if (time < 1f)
					tenebrisColor = Color.Lerp(ColorLib.TenebrisBeige, ColorLib.TenebrisMagenta, time);
				else if (time < 2f)
					tenebrisColor = Color.Lerp(ColorLib.TenebrisMagenta, ColorLib.TenebrisBlue, time - 1f);
				else
					tenebrisColor = Color.Lerp(ColorLib.TenebrisBlue, ColorLib.TenebrisBeige, time - 2f);

				tenebrisColor *= alpha;

				Main.spriteBatch.Draw(
					DTAssetLib.Square.Value,
					start,
					null,
					tenebrisColor,
					rotation,
					new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
					new Vector2(length, width),
					SpriteEffects.None,
					0f
				);
			}
            */

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Opus.DrawTextureOnProj(DTAssetLib.Star(1), Projectile, Color.White, true, 0f, 0.35f, 0.35f);

			return false;
		}


		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
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

			
			Projectile.rotation += Projectile.direction * 0.07f;

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 30 * 60);
		}

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }

    }
}