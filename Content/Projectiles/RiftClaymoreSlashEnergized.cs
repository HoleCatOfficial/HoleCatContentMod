using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class RiftClaymoreSlashEnergized : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 392;
            Projectile.height = 259;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 2) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                    Projectile.Kill();
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center;

            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;

            AnimateProjectile();

            if (player.HeldItem.type != ModContent.ItemType<RiftClaymore>())
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/RiftClaymorePowerStrike") with { PitchVariance = 0.2f }, target.Center);
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
        }
    }
}