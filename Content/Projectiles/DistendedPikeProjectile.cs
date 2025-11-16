using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.MeleeWeapons;
using System;
using Microsoft.Build.Evaluation;
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles
{
	public class DistendedPikeProjectile : ModProjectile
	{
		protected virtual float HoldoutRangeMin => 24f;
		protected virtual float HoldoutRangeMax => 220f;

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
		}

        private bool hasSpawned = false;
        private int ProjShootTimer = 0;
		public override bool PreAI() {
            ProjShootTimer++;
			Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
			int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

			player.heldProj = Projectile.whoAmI; // Update the player's held projectile id
            Vector2 ToMouse = Main.MouseWorld - Projectile.Center;

			// Reset projectile time left if necessary
			if (Projectile.timeLeft > duration) {
				Projectile.timeLeft = duration;
			}

			Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

			float halfDuration = duration * 0.5f;
			float progress;

			// Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
			if (Projectile.timeLeft < halfDuration) {
				progress = Projectile.timeLeft / halfDuration;
			}
			else {
				progress = (duration - Projectile.timeLeft) / halfDuration;
			}

			// Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
			Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
			
			// Apply proper rotation to the sprite.
			if (Projectile.spriteDirection == -1) {
				// If sprite is facing left, rotate 45 degrees
				Projectile.rotation += MathHelper.PiOver2;
			}
			else {
				// If sprite is facing right, rotate 135 degrees
				Projectile.rotation += MathHelper.PiOver2;
			}
			
            if (!hasSpawned && ProjShootTimer >= 10)
            {
                Vector2 direction = ToMouse.SafeNormalize(Vector2.UnitY);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt);
                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (direction * 6).RotatedByRandom(0.6f), ModContent.ProjectileType<DistendedPikeShard>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                }
                hasSpawned = true;
            }

			//Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// Avoid spawning dusts on dedicated servers
			if (!Main.dedServ) {
				if (Main.rand.NextBool(3)) {
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 1.2f);
				}

				if (Main.rand.NextBool(4)) {
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, Alpha: 128, Scale: 0.3f);
				}
			}
			return false; // Don't execute vanilla AI.
		}
	}
}