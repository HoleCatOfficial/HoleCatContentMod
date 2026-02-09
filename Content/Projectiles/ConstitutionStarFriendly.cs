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
using DestroyerTest.Content.Particles.Stellar;

namespace DestroyerTest.Content.Projectiles
{
	public class ConstitutionStarFriendly : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
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
			Projectile.width = 24;
			Projectile.height = 24;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
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

            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count);
					Color b = (MainColor * t) * Projectile.Opacity;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}

				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(2).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}

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

			UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion * 4f);

            DelayTimer++;

            Projectile.rotation += Projectile.direction * 0.1f;

            Lighting.AddLight(Projectile.Center, MainColor.ToVector3() * 0.2f);

			

			if (!StartKill)
			{
				if (Main.rand.NextBool(3))
				{
					PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, default, 0.5f);
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

			

				if (NPCTarget == null)
				{
					NPCTarget = FindClosestNPC(maxDetectRadius);
				}


				if (NPCTarget != null && !IsValidNPC(NPCTarget))
				{
					NPCTarget = null;
				}


				if (NPCTarget == null)
					return;

				float targetAngle = Projectile.AngleTo(NPCTarget.Center);
				if (HomingTime > 0)
				{
					Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * Projectile.velocity.Length();
				}

				if (!Flag1)
					{
						SoundEngine.PlaySound(SoundID.AbigailUpgrade, Projectile.Center);
						Flag1 = true;
					}

				// Acceleration
				float speed = Projectile.velocity.Length();
				float desiredSpeed = 20f; // your top speed
				float acceleration = 0.3f; // how quickly it ramps up
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
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs)
			{
				if (IsValidNPC(target))
				{

					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidNPC(NPC target)
		{
			return target.CanBeChasedBy();
		}

        public override bool? CanHitNPC(NPC target)
        {
            return !StartKill && DelayTimer >= 20;
        }


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
		}

        public override void OnKill(int timeLeft)
        {
			if (!StartKill)
			{
				SoundEngine.PlaySound(DTAssetLib.ConstitutionStarKill, Projectile.Center);
				Opus.NewParticleFloatAI(StellarParticleIndex.BloomRingSharp, Projectile.Center, Vector2.Zero, ColorLib.StellarFireGradientLooping(3f), 0.005f, 0.2f);
				DTUtils.ConstitutionStarExplosionEffects(Projectile);
			}
			else
			{
				SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);
				Opus.RadialSpreadParticle(StellarParticleIndex.ConstitutionParticle, 5, Projectile.Center, 1f, default, 1f, 2f, offset: Projectile.rotation);
			}
        }

    }

    public class ConstitutionStarFriendly_NoHoming : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
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

            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count);
					Color b = (MainColor * t) * Projectile.Opacity;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}

				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(2).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}

			Opus.DrawTextureOnProj(DTAssetLib.StarAura, Projectile, MainColor * Projectile.Opacity, false, Projectile.velocity.ToRotation(), Projectile.scale, Projectile.scale);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(DTAssetLib.ColorlessStar, Projectile, Color.White * Projectile.Opacity, true, Projectile.rotation, Projectile.scale, Projectile.scale);

            return false;
        }

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 400;
        public SoundStyle Chase = new SoundStyle($"DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionStar/Chase") { PitchVariance = 1f, MaxInstances = 0 };

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

            

            Projectile.rotation += Projectile.direction * 0.1f;

			UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion * 4f);

            Lighting.AddLight(Projectile.Center,  MainColor.ToVector3() * 0.2f);
			
			if (!StartKill)
			{
				if (Main.rand.NextBool(3))
				{
					PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, default, 0.5f);
				}
			}

			if (StartKill)
			{
				Projectile.velocity *= 0.97f;
				Projectile.scale *= 0.97f;
				Projectile.Opacity -= 0.01f;
			}
        }
		

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
		}

        public override void OnKill(int timeLeft)
        {
			if (!StartKill)
			{
				SoundEngine.PlaySound(DTAssetLib.ConstitutionStarKill, Projectile.Center);
				PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero,  ColorLib.StellarFireGradientLooping(3f), 0.005f);
				Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0,  ColorLib.StellarFireGradientLooping(3f), 2f);
				DTUtils.ConstitutionStarExplosionEffects(Projectile);
			}
			else
			{
				SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);
				Opus.RadialSpreadParticle(StellarParticleIndex.ConstitutionParticle, 5, Projectile.Center, 1f, default, 1f, 2f, offset: Projectile.rotation);
			}
        }
    }
}