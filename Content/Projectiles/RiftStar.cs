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

namespace DestroyerTest.Content.Projectiles
{
	/// <summary>
	/// Multipurpose Projectile.
	/// <para/> Projectile ai slots 0 and 1 should not be set to anything when spawning, as they store NPC and Player values respectively.
	/// <para/> Projectile ai slot 2 controls whether the projectile is friendly or harmful.
	/// </summary>
	public class RiftStar : ModProjectile
	{
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

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
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.Rift;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

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

				float time = (Main.GlobalTimeWrappedHourly + i * 0.05f) % 3f;

				Color RiftColor;
				if (time < 1f)
					RiftColor = Color.Lerp(ColorLib.Rift, ColorLib.DarkRift4, time);
				else if (time < 2f)
					RiftColor = Color.Lerp(ColorLib.DarkRift4, ColorLib.DarkRift3, time - 1f);
                else if (time < 3f)
					RiftColor = Color.Lerp(ColorLib.DarkRift3, ColorLib.DarkRift2, time - 2f);
                else if (time < 4f)
					RiftColor = Color.Lerp(ColorLib.DarkRift2, ColorLib.DarkRift1, time - 3f);
                else if (time < 5f)
					RiftColor = Color.Lerp(ColorLib.DarkRift1, ColorLib.DarkRift2, time - 4f);
                else if (time < 6f)
					RiftColor = Color.Lerp(ColorLib.DarkRift2, ColorLib.DarkRift3, time - 5f);
				else
                    RiftColor = Color.Lerp(ColorLib.DarkRift3, ColorLib.DarkRift4, time - 6f);

				RiftColor *= alpha;

				Main.spriteBatch.Draw(
					DTAssetLib.Square.Value,
					start,
					null,
					RiftColor,
					rotation,
					new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
					new Vector2(length, width),
					SpriteEffects.None,
					0f
				);
			}

			Utility.DrawGlowOnProj(Projectile, lightColor, true);

			Utility.ReturnToDefaultDrawing(spriteBatch);
			
			Utility.DrawTextureOnProj(DTAssetLib.RiftStar, Projectile, Color.White, true, 0f, 0.35f, 0.35f);

			return false;
		}


		/// <summary>
		/// Controls whether the Projectile is Hostile or Friendly.
		/// <para/> 1 = Friendly, 2 = Hostile
		/// <para/> Attempting to return an invalid value will kill the projectile.
		/// </summary>
		public int Mode;

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;

		public override void AI()
		{
			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			DelayTimer++;
			Mode = (int)Projectile.ai[2];
			Projectile.rotation += Projectile.direction * Main.rand.NextFloat(0.01f, 0.07f);

            if (Main.rand.NextBool(12))
            {
                PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Projectile.Center + Main.rand.NextVector2Circular(10, 10), Vector2.Zero, ColorLib.Rift, 0.1f);
            }
			if (Mode > 4 || Mode <= 0)
                {
                    Projectile.Kill();
                    //throw new Exception("Non-Fatal Error in Oil Projectile Targeting. Value must be 1 or 2.");
                    Mod.Logger.Warn("OilProjectile: Invalid Mode in ai[2]. Expected 1 or 2.");
                }

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

			if (DelayTimer < 20)
			{
				DelayTimer += 1;
				return;
			}

			float maxDetectRadius = 2800f;

			if (Mode == 1)
			{
				Projectile.friendly = true;
				Projectile.hostile = false;

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
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * Projectile.velocity.Length();

				// Acceleration
				float speed = Projectile.velocity.Length();
				float desiredSpeed = 20f; // your top speed
				float acceleration = 0.3f; // how quickly it ramps up
				if (speed < desiredSpeed)
					speed += acceleration;
				Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
			}
			if (Mode == 2)
			{
				Projectile.friendly = false;
				Projectile.hostile = true;

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

				// Acceleration
				float speed = Projectile.velocity.Length();
				float desiredSpeed = 18f;
				float acceleration = 0.25f;
				if (speed < desiredSpeed)
					speed += acceleration;
				Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;

			}
			if (Mode == 3)
			{
				Projectile.friendly = true;
				Projectile.hostile = false;
			}
			if (Mode == 4)
			{
				Projectile.friendly = false;
				Projectile.hostile = true;
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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Mode == 1 || Mode == 3)
			{
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/RiftCharge") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.25f }, target.Center);
				target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Mode == 2 || Mode == 4)
			{
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/RiftCharge") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.25f }, target.Center);
				target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
			}
		}

        public override void OnKill(int timeLeft)
        {
            PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.05f);
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<RiftDust>(), Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, default, 2f);
        }

    }
}