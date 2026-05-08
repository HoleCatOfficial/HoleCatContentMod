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

namespace DestroyerTest.Content.Projectiles
{
	public class RiftStarHostile : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		private Player PLRTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
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
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.Rift;
			trailOffset += 0.04f;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!OptCfg.DisableExcessTrails)
            {
				Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

				if (TrailPositions.Count > 1)
				{
					List<ColoredVertex> ve = new List<ColoredVertex>();
					float a = 0;

					for (int i = TrailPositions.Count - 1; i > 0; i--)
					{
						float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
						Color b = lightColor * t;

						Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
						Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 32;
						Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 32;

						/*
						ve.Add(new ColoredVertex(
							TrailPositions[i] - Main.screenPosition + offset,
							new Vector3(t, 1, 1),
							b));

						ve.Add(new ColoredVertex(
							TrailPositions[i] - Main.screenPosition + offset2,
							new Vector3(t, 0, 1),
							b));
							*/
							
						DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
					}


					GraphicsDevice gd = Main.graphics.GraphicsDevice;
					if (ve.Count >= 3)
					{
						gd.Textures[0] = DTAssetLib.Streak(5).Value;
						gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
					}
				}
			}

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);
			
			Opus.DrawTextureOnProj(DTAssetLib.RiftStar, Projectile, ColorLib.Rift, true, 0f, 0.9f, 0.9f);

			return false;
		}



		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 400;

		public override void AI()
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

			DelayTimer++;
			Projectile.rotation += Projectile.direction * Main.rand.NextFloat(0.01f, 0.07f);

            if (Main.rand.NextBool(12))
            {
                PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Projectile.Center + Main.rand.NextVector2Circular(10, 10), Vector2.Zero, ColorLib.Rift, 0.1f);
            }

			Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3() * 0.2f);

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

            float targetAngle = Projectile.AngleTo(PLRTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * Projectile.velocity.Length();

			float speed = Projectile.velocity.Length();
			float desiredSpeed = 35f;
			float acceleration = 0.3f;
			if (speed < desiredSpeed)
				speed += acceleration;
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
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
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/RiftCharge") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.25f }, target.Center);
			target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, default, 2f);
            }
        }
    }
}