using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	// This Example show how to implement simple homing projectile
	// Can be tested with ExampleCustomAmmoGun
	public class ContinuumStar : ModProjectile
	{
		// Store the target NPC using Projectile.ai[0]
		private NPC HomingTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public override void SetDefaults()
		{
			Projectile.width = 33; // The width of projectile hitbox
			Projectile.height = 33; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.4f; // How much light emit around the projectile
			Projectile.timeLeft = 240; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.StellarColor;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();
			Texture2D projectileTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/StarParticle2").Value;
			Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

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

				// Offset disco values by i + time, so it looks like bands traveling
				float shift = (Main.GlobalTimeWrappedHourly * 60f + i * 10f) % 255f;

				byte r = (byte)((Math.Sin((shift + 0) * 0.0245f) * 127 + 128) / 2);     // similar to Main.DiscoR /2
				byte g = (byte)((Math.Sin((shift + 85) * 0.0245f) * 127 + 128) / 1.25); // offset phase
				byte b = (byte)((Math.Sin((shift + 170) * 0.0245f) * 127 + 128) / 1.5); // offset phase

				Color rainbowColor = new Color(r, g, b) * alpha;

				Main.spriteBatch.Draw(
					pixel,
					start,
					null,
					rainbowColor,
					rotation,
					new Vector2(pixel.Width / 2, pixel.Height / 2),
					new Vector2(length, width),
					SpriteEffects.None,
					0f
				);
			}

			Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleParticle").Value;
			Main.EntitySpriteDraw(
				glowTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				glowTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			// Draw the base projectile using the default drawing system (Deferred)
			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				Color.White,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale * 0.6f,
				SpriteEffects.None,
				0
			);

			Utility.ReturnToDefaultDrawing(spriteBatch);

			
			return false; // Let the default system handle the base projectile drawing
		}

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

			Lighting.AddLight(Projectile.Center, ColorLib.StellarColor.ToVector3() * 1.0f);

			if (DelayTimer < 10)
			{
				DelayTimer += 1;
				return;
			}

			float maxDetectRadius = 1400f; // The maximum radius at which a projectile can detect a target

			// First, we find a homing target if we don't have one
			if (HomingTarget == null)
			{
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			// If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
			if (HomingTarget != null && !IsValidTarget(HomingTarget))
			{
				HomingTarget = null;
			}

			// If we don't have a target, don't adjust trajectory
			if (HomingTarget == null)
				return;

			// If found, we rotate the projectile velocity in the direction of the target.
			// We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(18)).ToRotationVector2() * length;
		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs
			foreach (var target in Main.ActiveNPCs)
			{
				// Check if NPC able to be targeted. 
				if (IsValidTarget(target))
				{
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidTarget(NPC target)
		{
			// This method checks that the NPC is:
			// 1. active (alive)
			// 2. chaseable (e.g. not a cultist archer)
			// 3. max life bigger than 5 (e.g. not a critter)
			// 4. can take damage (e.g. moonlord core after all it's parts are downed)
			// 5. hostile (!friendly)
			// 6. not immortal (e.g. not a target dummy)
			// 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
			return target.CanBeChasedBy();
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 120);

		}

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
			Dust.NewDust(
				Projectile.Center,
				Projectile.width,
				Projectile.height,
				DustID.TintableDustLighted,
				Projectile.velocity.X * 0.4f,
				Projectile.velocity.Y * 0.4f,
				0,
				ColorLib.RainbowGradient,
				1f
			);

        }
		

	}
}