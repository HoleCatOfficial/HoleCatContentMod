using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
	public class ConstitutionStarHostile : ModProjectile
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
            ProjectileID.Sets.TrailCacheLength[Type] = 400;
            ProjectileID.Sets.TrailingMode[Type] = 3;

        }

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 420;
			Projectile.tileCollide = false;
		}

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
        public float trailOffset = 0;
		public Color MainColor = Color.White;
		public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.ConstitutionStarTrail.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, MainColor, trailOffset, 4);

            Opus.DrawTextureOnProj(DTAssetLib.StarAura, Projectile, MainColor * Projectile.Opacity, false, Projectile.velocity.ToRotation(), Projectile.scale, Projectile.scale);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(DTAssetLib.ColorlessStar, Projectile, Color.White * Projectile.Opacity, true, Projectile.rotation, Projectile.scale, Projectile.scale);

            return false;
        }

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 400;
        public SoundStyle Chase = new SoundStyle($"DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionStar/Chase") { PitchVariance = 1f, MaxInstances = 0 };

        public bool Flag1 = false;
        public int HomingTime = 60;

		public int Lifetime = 300;
		public int Time = 0;

		public bool StartKill = false;
		public void UpdateLerpTime()
		{
			Time++;

			if (Time > Lifetime)
			{
				StartKill = true;
			}
		}
		public float LifetimeCompletion
		{
			get
			{
				if (Lifetime <= 0)
				{
					return 0f;
				}

				return (float)Time / (float)Lifetime;
			}
		}

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

            Projectile.rotation += Projectile.direction * 0.1f;

			UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion);

            Lighting.AddLight(Projectile.Center,  MainColor.ToVector3() * 0.2f);

            if (!StartKill)
            {
                if (Main.rand.NextBool(3))
                {
                    ConstitutionParticle Particle = new();
                    Particle.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, 1f, 60);
                    ParticleEngine.BehindProjectiles.Add(Particle);
                }
            

				if (DelayTimer < 20)
				{
					DelayTimer += 1;
					return;
				}

				if (HomingTime > 0 && DelayTimer >= 20)
				{
					HomingTime--;
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
				if (HomingTime > 0)
				{
					Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * Projectile.velocity.Length();
				}

				if (!Flag1)
				{
					SoundEngine.PlaySound(SoundID.AbigailUpgrade, Projectile.Center);
					Flag1 = true;
				}

				// Acceleration
				float speed = Projectile.velocity.Length();
				float desiredSpeed = 18f;
				float acceleration = 0.25f;
				if (HomingTime > 0)
				{
					if (speed < desiredSpeed)
						speed += acceleration;
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				}
			}

			if (StartKill)
			{
				Projectile.velocity *= 0.97f;
				Projectile.scale *= 0.97f;
				Projectile.Opacity -= 0.01f;
			}
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
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
		}

        public override void OnKill(int timeLeft)
        {
			if (!StartKill)
			{
				SoundEngine.PlaySound(DTAssetLib.ConstitutionStarKill, Projectile.Center);
				StellarParticleUtils.BloomRing(Projectile.Center, 0.5f, ParticleEngine.BehindProjectiles);
				Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0,  ColorLib.StellarFireGradientLooping(), 2f);
				DTUtils.ConstitutionStarExplosionEffects(Projectile);
			}
			else
			{
				SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);
				
			}
        }

    }

    public class ConstitutionStarHostile_NoHoming : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Type] = 400;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 420;
			Projectile.tileCollide = false;
		}

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
        public float trailOffset = 0;
		public Color MainColor = Color.White;
		public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.ConstitutionStarTrail.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, MainColor, trailOffset, 4);

            Opus.DrawTextureOnProj(DTAssetLib.StarAura, Projectile, MainColor * Projectile.Opacity, false, Projectile.velocity.ToRotation(), Projectile.scale, Projectile.scale);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(DTAssetLib.ColorlessStar, Projectile, Color.White * Projectile.Opacity, true, Projectile.rotation, Projectile.scale, Projectile.scale);

            return false;
        }

		public int Lifetime = 300;
		public int Time = 0;

		public bool StartKill = false;
		public void UpdateLerpTime()
		{
			Time++;

			if (Time > Lifetime)
			{
				StartKill = true;
			}
		}
		public float LifetimeCompletion
		{
			get
			{
				if (Lifetime <= 0)
				{
					return 0f;
				}

				return (float)Time / (float)Lifetime;
			}
		}

		public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation += Projectile.direction * 0.1f;

			UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion);

            Lighting.AddLight(Projectile.Center,  MainColor.ToVector3() * 0.2f);

			if (!StartKill)
			{
				if (Main.rand.NextBool(3))
				{
					ConstitutionParticle Particle = new();
					Particle.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, 1f, 60);
					ParticleEngine.BehindProjectiles.Add(Particle);
				}
			}

			if (StartKill)
			{
				Projectile.velocity *= 0.97f;
				Projectile.scale *= 0.97f;
				Projectile.Opacity -= 0.01f;
			}
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
		}

        public override void OnKill(int timeLeft)
        {
            if (!StartKill)
            {
                SoundEngine.PlaySound(DTAssetLib.ConstitutionStarKill, Projectile.Center);
                StellarParticleUtils.BloomRing(Projectile.Center, 0.5f, ParticleEngine.BehindProjectiles);
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.StellarFireGradientLooping(), 2f);
                DTUtils.ConstitutionStarExplosionEffects(Projectile);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);

            }
        }

    }
}