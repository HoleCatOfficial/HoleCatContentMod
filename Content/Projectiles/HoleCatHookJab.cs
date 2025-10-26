
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class HoleCatHookJab : ModProjectile
    {
        // Define the range of the Spear Projectile. These are overridable properties, in case you'll want to make a class inheriting from this one.
        public SoundStyle Jab = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/QuickSwing", 4) with { MaxInstances = 0 };
		protected virtual float HoldoutRangeMin => 24f;
		protected virtual float HoldoutRangeMax => 600f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.hide = false;
            Projectile.width = 200;
            Projectile.height = 200;
        }

        public bool Flag1 = false;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            if (!Flag1)
            {
                SoundEngine.PlaySound(Jab, Projectile.Center);
                Flag1 = true;
            }

            Vector2 Dustpos;
            Vector2 DustPos = Vector2.One;
            for (int f = 0; f < 28; f++)
            {
                Dustpos = Projectile.Center + new Vector2(50, -10);
                DustPos = Dustpos.RotatedBy(Projectile.rotation);
            }


            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            //Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 perpendicular = direction.RotatedBy(MathHelper.PiOver2); // 90° perpendicular

            // progress goes 0→1→0, so we can remap it for a smooth oscillation
            float arc = (float)Math.Sin(progress * MathHelper.Pi); // smooth arc up and down (0→1→0)
            float arcMagnitude = 24f; // how wide the oval path is

            // base forward motion
            Vector2 forwardOffset = Vector2.SmoothStep(direction * HoldoutRangeMin, direction * HoldoutRangeMax, progress);
            // add the perpendicular arc
            Vector2 curveOffset = perpendicular * arc * arcMagnitude;
            curveOffset *= Projectile.direction; // make arc face the correct way

            // final position
            Projectile.Center = (player.MountedCenter + forwardOffset + curveOffset) + new Vector2(0, -30);
            for (int f = 0; f < 3; f++)
            {
                Dust.NewDustPerfect(DustPos, DustID.TintableDustLighted, Vector2.Zero, 100, ColorLib.HoleCatFireGradient, 6);
            }

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                //Projectile.rotation += MathHelper.ToRadians(135f);
                Projectile.rotation += MathHelper.ToRadians(45f);
            }

            return false; // Don't execute vanilla AI.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            int slot = (int)Projectile.ai[1]; // retrieve which slot created it
            if (slot < 0 || slot >= player.inventory.Length)
                return; // safety check

            if (player.inventory[slot].ModItem is HoleCatHook hook)
            {
                hook.jabHitCount++;
                if (hook.jabHitCount % 10 == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatHookFreeze"));
                    target.AddBuff(ModContent.BuffType<MobilityHex>(), 120);
                }
            }
        }

	}
}