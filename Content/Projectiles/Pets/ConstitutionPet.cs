using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Pets
{
    public class ConstitutionPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public float Randomizer => Projectile.localAI[0];
        private const int IdleModeSwitchTime = 480; // every 8 seconds

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++; // timer

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<ConstitutionPetBuff>());

            if (player.HasBuff(ModContent.BuffType<ConstitutionPetBuff>()))
                Projectile.timeLeft = 2;

            // Choose an idle style every few seconds.
            int mode = (int)((Projectile.ai[0] / IdleModeSwitchTime) % 3);
            DoIdleMovement(player, mode);
            KeepUp(1200f, 2400f, player);
        }

        private void DoIdleMovement(Player player, int mode)
        {
            Vector2 idlePos = player.Center;
            float time = Projectile.ai[0] / 60f; // seconds in float
            float offsetRadius = 100f; // base distance from player

            Vector2 targetPos = idlePos;

            switch (mode)
            {
                case 0:
                    // Sweep left and right behind the player 2–3 times, deltaY changes slightly
                    float sweep = (float)Math.Sin(time * 2f) * 120f;
                    float verticalOffset = -40f + (float)Math.Sin(time * 0.3f) * 20f;
                    targetPos = player.Center + new Vector2(-sweep, verticalOffset);
                    break;

                case 1:
                    // Circle the player once per 5 seconds
                    float angle = time * MathHelper.TwoPi / 5f;
                    targetPos = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * offsetRadius;
                    break;

                case 2:
                    // Hover overhead with a subtle figure-eight
                    float figX = (float)Math.Sin(time * 1.5f) * 50f;
                    float figY = (float)Math.Sin(time * 3f) * 25f - 70f;
                    targetPos = player.Center + new Vector2(figX, figY);
                    break;
            }

            // Smoothly move toward target position
            Vector2 desiredVelocity = (targetPos - Projectile.Center) * 0.08f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.rotation = Projectile.velocity.ToRotation() * 0.7f;
        }

        public SoundStyle TP = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionBossKill")
        {
            PitchRange = (0.5f, 1f),
            MaxInstances = 0
        };

        private void KeepUp(float distSpeed, float distTeleport, Player master)
        {
            float dist = Projectile.Distance(master.Center);

            // If close enough, do nothing
            if (dist < distSpeed) return;

            // Speed up if lagging but not too far
            if (dist < distTeleport)
            {
                int maxSpeed = 35;
                Vector2 toPlayer = master.Center - Projectile.Center;
                float length = toPlayer.Length();
                if (length > 0)
                {
                    toPlayer /= length;
                    float speed = MathHelper.Clamp(length / 12f, 8f, maxSpeed);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toPlayer * speed, 0.1f);
                }
                return;
            }

            // Too far — teleport back
            if (dist > distTeleport)
            {
                SoundEngine.PlaySound(TP, Projectile.Center);
                Projectile.Center = master.Center;
                Projectile.velocity *= 0.1f;
            }
        }
    }
}
