using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;


namespace DestroyerTest.Content.Projectiles.Pets
{
    [AutoloadGlowmask]
    public class RosePet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 6;
        }
        public SoundStyle TP = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/SoulSummon")
        {
            PitchRange = (0.5f, 1f),
            MaxInstances = 0
        };

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 86;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(TP, Projectile.Center);
            for(int f = 0; f < 8; f++)
            {
                Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(3, 3);
                Vector2 Dir = Projectile.Center - Outer;
                PRTLoader.NewParticle(PRTLoader.GetParticleID<TormentedSoulParticle>(), Projectile.Center, Dir, default, Main.rand.NextFloat(0.15f, 2.5f));
                PRTLoader.NewParticle(PRTLoader.GetParticleID<TormentedSoulParticle2>(), Projectile.Center, Dir, default, Main.rand.NextFloat(0.15f, 2.5f));
            }
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
        
        public float Randomizer => Projectile.localAI[0];
        private const int IdleModeSwitchTime = 480;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0] += 0.5f;

            AnimateProjectile();

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<RosePetBuff>());

            if (player.HasBuff(ModContent.BuffType<RosePetBuff>()))
                Projectile.timeLeft = 2;

            int mode = (int)((Projectile.ai[0] / IdleModeSwitchTime) % 3);
            DoIdleMovement(player, mode);
            KeepUp(1200f, 2400f, player);
        }

        private void DoIdleMovement(Player player, int mode)
        {
            Vector2 idlePos = player.Center;
            float time = Projectile.ai[0] / 60f;
            float offsetRadius = 100f;

            Vector2 targetPos = idlePos;

            switch (mode)
            {
                case 0:
                    float sweep = (float)Math.Sin(time * 2f) * 120f;
                    float verticalOffset = -40f + (float)Math.Sin(time * 0.3f) * 20f;
                    targetPos = player.Center + new Vector2(-sweep, verticalOffset);
                    break;

                case 1:
                    float angle = time * MathHelper.TwoPi / 5f;
                    targetPos = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * offsetRadius;
                    break;

                case 2:
                    float figX = (float)Math.Sin(time * 1.5f) * 50f;
                    float figY = (float)Math.Sin(time * 3f) * 25f - 70f;
                    targetPos = player.Center + new Vector2(figX, figY);
                    break;
            }

            Projectile.spriteDirection = 1;

            Vector2 desiredVelocity = (targetPos - Projectile.Center) * 0.08f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

            float Ob = Projectile.velocity.ToRotation() * 0.05f;
            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.rotation = float.Lerp(Projectile.rotation, Ob, 0.15f);
        }

       

        private void KeepUp(float distSpeed, float distTeleport, Player master)
        {
            float dist = Projectile.Distance(master.Center);

            if (dist < distSpeed) return;

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

            if (dist > distTeleport)
            {
                for(int f = 0; f < 8; f++)
                {
                    Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(3, 3);
                    Vector2 Dir = Projectile.Center - Outer;
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<TormentedSoulParticle>(), Projectile.Center, Dir, default, Main.rand.NextFloat(0.15f, 2.5f));
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<TormentedSoulParticle2>(), Projectile.Center, Dir, default, Main.rand.NextFloat(0.15f, 2.5f));
                }
                SoundEngine.PlaySound(TP, Projectile.Center);
                Projectile.Center = master.Center;
                Projectile.velocity *= 0.1f;
            }
        }
    }
}
