using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using Terraria.Utilities.Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using OpusLib;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class BlackDiamondProjectile : ModProjectile
    {
        // Define the range of the Spear Projectile. These are overridable properties, in case you'll want to make a class inheriting from this one.
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 60f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
        }

        public float ShineOpacity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.5f) * ShineOpacity, Projectile.velocity.ToRotation() + MathHelper.PiOver2, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(1f, 2f), SpriteEffects.None, 0f);
            return true;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            Vector2 d = Main.MouseWorld - player.MountedCenter;
            player.SetCompositeArmFront(Projectile.active, Player.CompositeArmStretchAmount.ThreeQuarters, d.ToRotation() - MathHelper.PiOver2);

            

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            //Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
                ShineOpacity = MathHelper.Lerp(1f, 0f, progress.Inverse());
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
                ShineOpacity = MathHelper.Lerp(1f, 0f, progress.Inverse());
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.PiOver2;
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.PiOver2;
            }


            //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Avoid spawning dusts on dedicated servers
            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, 50, ColorLib.TenebrisBlue, 1f);
                }

                Dust T = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.5f, 0, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.5f) * ShineOpacity, 1f);
                T.noGravity = true;
            }
            Projectile.rotation = d.ToRotation();

            return false; // Don't execute vanilla AI.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { PitchVariance = 0.2f });
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { PitchVariance = 0.2f });
            //Opus.RadialSpreadParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 4, Projectile.Center, 1f, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.5f), 0.5f, 1.5f, offset: Projectile.velocity.ToRotation());
            //Opus.RadialSpreadParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 4, Projectile.Center, 1f, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.95f), 0.1f, 0.75f, offset: Projectile.velocity.ToRotation() + MathHelper.PiOver4);
            
        }
    }
}