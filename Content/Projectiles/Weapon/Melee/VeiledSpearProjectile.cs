using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Particles;
using GlowmaskHelper.Content;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    [AutoloadGlowmask]
    public class VeiledSpearProjectile : ModProjectile
    {
        // Define the range of the Spear Projectile. These are overridable properties, in case you'll want to make a class inheriting from this one.
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 45f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
        }

        public float ShineOpacity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.Sparkle(2).Value, Projectile.Center - Main.screenPosition, null, ColorLib.DarkRift3 * ShineOpacity, Projectile.velocity.ToRotation(), DTAssetLib.Sparkle(2).Value.Size() / 2, new Vector2(1f, 2f), SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            //var Glow = Projectile.GetGlowTexture("DestroyerTest/Content/Projectiles/Weapon/Melee", "VeiledSpearProjectile");
            //Main.EntitySpriteDraw(Glow.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Glow.Value.Size() / 2, new Vector2(1f, 2f), SpriteEffects.None);
            return true;
        }

        bool flag1 = false;
        public void ProjectileSpawn()
        {
            if (flag1)
            {
                return;
            }

            SoundEngine.PlaySound(DTAssetLib.ChargeBreak, Projectile.Center);
            Vector2 d = Main.MouseWorld - player.MountedCenter;
            d.Normalize();
            Projectile.NewProjectile(Projectile.GetSource_Misc("SpearFullExtension"), Projectile.Center, d * 12, ModContent.ProjectileType<RiftSpark>(), (int)(Projectile.damage * 0.25f), 16, Projectile.owner);
            flag1 = true;
        }

        Player player => Main.player[Projectile.owner];

        public override bool PreAI()
        {
            // Since we access the owner player instance so much, it's useful to create a helper local variable for this
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
                ProjectileSpawn();
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
                Dust T = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.5f, 0, ColorLib.Rift * ShineOpacity, 1f);
                T.noGravity = true;
            }
            Projectile.rotation = d.ToRotation();

            return false; // Don't execute vanilla AI.
        }

        public override void AI()
        {
            Vector2 d = Main.MouseWorld - player.MountedCenter;
            Projectile.rotation = d.ToRotation() - MathHelper.PiOver4;
        }
        

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { PitchVariance = 0.2f });
            SoundEngine.PlaySound(DTAssetLib.Zap with { PitchVariance = 0.2f });
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 240);
        }
    }
}
