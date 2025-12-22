using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Terraria.Audio;
using DestroyerTest.Content.Magic;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class TenebrousTradewindsHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 20; // This projectile has 4 frames.
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2000; // persistent
        }

        private void AnimateProjectile() {
            if (++Projectile.frameCounter >= 1) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public int ShootTimer = 0;

        public override void AI()
        {
            SoundStyle Shoot = new SoundStyle($"DestroyerTest/Assets/Audio/TTUse") with
            {
                PitchVariance = 1.0f,
                MaxInstances = 0
            };
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<TenebrousTradewinds>() && player.channel == true)
            {
                AnimateProjectile();

                float holdDistance = 15f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                Projectile.Center = desiredPos;

                // Rotate to face the cursor
                Projectile.rotation = toCursor.ToRotation();

                if (player.direction == -1)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.direction = toCursor.X > 0 ? 1 : -1;


                if (Main.GameUpdateCount % 10 == 0)
                {
                    SoundEngine.PlaySound(Shoot);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -10).RotatedByRandom(1f), ModContent.ProjectileType<TenebrousLightDart>(), 280, 6f, Projectile.owner);
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

    }
}