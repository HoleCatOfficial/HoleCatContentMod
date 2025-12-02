using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
	// This Example show how to implement simple homing projectile
	// Can be tested with ExampleCustomAmmoGun
	public class FrostBurst : ModProjectile
	{
		// Store the target NPC using Projectile.ai[0]
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10; 

			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.SkyBlue;

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawGlowOnProj(Projectile, lightColor, false, 0);

			for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(0.15f, 0.0005f, progress);
				Color color = lightColor;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					TrailPositions[i] - Main.screenPosition,
					null,
					color,
					Projectile.rotation,
					DTAssetLib.FeatheredCircle.Value.Size() / 2,
					scale,
					SpriteEffects.None,
					0
				);
			}

			Main.EntitySpriteDraw(
				DTAssetLib.FeatheredCircle.Value,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				DTAssetLib.FeatheredCircle.Value.Size() / 2,
				0.15f,
				SpriteEffects.None,
				0
			);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 15;
        }

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;
		
		public override void AI()
		{
			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 1.0f);
			
			if (DelayTimer < 15)
			{
				DelayTimer += 1;
				return;
			}
			
			float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target

			// First, we find a homing target if we don't have one
			if (HomingTarget == null) {
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			// If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
			if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
				HomingTarget = null;
			}

			// If we don't have a target, don't adjust trajectory
			if (HomingTarget == null)
				return;

			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(10)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs) {
				if (IsValidTarget(target)) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidTarget(NPC target) {
			return target.CanBeChasedBy();
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			Player player = Main.player[Main.myPlayer];  // Accessing the current player
			target.AddBuff(ModContent.BuffType<HaepiensBlizzard>(), 60);
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/IceImpact1"));
			PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom1>(), target.Center, Vector2.Zero, Color.SkyBlue, 1);
		}
	}
}