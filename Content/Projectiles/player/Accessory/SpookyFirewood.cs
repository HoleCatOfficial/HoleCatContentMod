using System;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class SpookyFirewood : ModProjectile
    {
        public int Variant = Main.rand.Next(1, 4);
        public int Dir = Main.rand.Next(1, 3);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        private float orbitAngle;
        private const float OrbitRadius = 80f;

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Variant;
        }

        public int[] FireVariants = new int[]
        {
            ProjectileID.GreekFire1,
            ProjectileID.GreekFire2,
            ProjectileID.GreekFire3
        };

        public override void AI()
        {
            // Determine orbit center based on ai slots
            Vector2 centerPoint = Vector2.Zero;

            // Priority: Projectile > Player > NPC
            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxProjectiles)
            {
                Projectile targetProj = Main.projectile[(int)Projectile.ai[0]];
                if (targetProj.active)
                    centerPoint = targetProj.Center;
            }
            else if (Projectile.ai[1] >= 0 && Projectile.ai[1] < Main.maxPlayers)
            {
                Player targetPlayer = Main.player[(int)Projectile.ai[1]];
                if (targetPlayer.active)
                    centerPoint = targetPlayer.Center;
            }
            else if (Projectile.ai[2] >= 0 && Projectile.ai[2] < Main.maxNPCs)
            {
                NPC targetNPC = Main.npc[(int)Projectile.ai[2]];
                if (targetNPC.active)
                    centerPoint = targetNPC.Center;
            }

            // If no valid target, kill the projectile
            if (centerPoint == Vector2.Zero)
            {
                Projectile.Kill();
                return;
            }

            // Shared angular reference
            float baseAngle = (Main.GameUpdateCount * 0.03f) % MathHelper.TwoPi;

            // Sibling management
            var sameType = Main.projectile
                .Where(p => p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                .OrderBy(p => p.identity)
                .ToList();

            int index = sameType.IndexOf(Projectile);
            int total = sameType.Count;
            float spacing = MathHelper.TwoPi / Math.Max(1, total);

            // Orbit math
            float angle = baseAngle + index * spacing;
            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * OrbitRadius;

            // Smoothly move toward orbit position
            Vector2 desiredPosition = centerPoint + offset;
            Projectile.velocity = (desiredPosition - Projectile.Center) * 0.1f;

            // Rotation direction
            Projectile.rotation += Dir == 1 ? 0.01f : -0.01f;
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NodeExplode"), Projectile.Center);
            Opus.RadialSpreadProjectileRandom(FireVariants[Main.rand.Next(FireVariants.Length)], Main.rand.Next(8, 13), Projectile.Center, Projectile.damage / 2, 5, 10);
        }
    }
}