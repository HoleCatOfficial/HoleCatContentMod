using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Linq;
using System.Xml;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class HeartNode : ModProjectile
    {
        private struct HeartConnection
        {
            public int targetIndex;
            public int projDurability;
            public int npcDurability;
            public bool active;
        }

        private HeartConnection[] connections = new HeartConnection[3];
        private bool initialized = false;
        private int hitCooldown = 0;
        private int spawnGraceTimer = 60; 
        private bool dying = false;

        private SoundStyle kill = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/IceMagicImpact", 3) with { PitchVariance = 0.4f };
        private SoundStyle DurabilityDown = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionStar/Kill", 14) with { PitchVariance = 0.4f };


        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
           
        }

        public override void AI()
        {
            AnimateProjectile();
            Projectile.velocity = Vector2.Zero;

            if (!initialized || (spawnGraceTimer > 0 && spawnGraceTimer % 15 == 0))
            {
                InitializeConnections();
                initialized = true;
            }

            bool anyActive = false;

            for (int i = 0; i < connections.Length; i++)
            {
                if (!connections[i].active) continue;

                Projectile node = Main.projectile[connections[i].targetIndex];
                if (!node.active || node.type != ModContent.ProjectileType<HeartNode>())
                {
                    connections[i].active = false;
                    continue;
                }

                float distance = Vector2.Distance(Projectile.Center, node.Center);
                if (distance > 700f)
                {
                    connections[i].active = false;
                    continue;
                }

                anyActive = true;
                DrawConnection(Projectile.Center, node.Center);

                HandleProjectileHits(i, node);
                HandlePlayerHits(i, node);
            }

            if (spawnGraceTimer > 0)
            {
                spawnGraceTimer--;
            }
            else if (!anyActive && !dying)
            {
                BeginDeathCountdown();
                return;
            }



            if (hitCooldown < 40)
                hitCooldown++;

            if (Projectile.timeLeft == 10)
            {

            }
        }

        private void InitializeConnections()
        {
            var others = Main.projectile
                .Where(p => p.active && p.type == ModContent.ProjectileType<HeartNode>() && p.whoAmI != Projectile.whoAmI)
                .OrderBy(p => Vector2.Distance(Projectile.Center, p.Center))
                .Take(3)
                .ToList();

            for (int i = 0; i < others.Count; i++)
            {
                connections[i] = new HeartConnection
                {
                    targetIndex = others[i].whoAmI,
                    projDurability = 10,
                    npcDurability = 15,
                    active = true
                };
            }
        }

        private void DrawConnection(Vector2 start, Vector2 end)
        {
            float distance = Vector2.Distance(start, end);
            int pointCount = (int)(distance / 50f);
            float[] randomTs = new float[pointCount];
            for (int i = 0; i < pointCount; i++) randomTs[i] = Main.rand.NextFloat();
            Array.Sort(randomTs);

            for (int i = 0; i < pointCount; i++)
            {
                float t = randomTs[i];
                Vector2 point = Vector2.Lerp(end, start, t);
                Dust.NewDustPerfect(point, DustID.TintableDustLighted, Vector2.Zero, 0, Color.Red, 1f);
                //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), point, Vector2.Zero, Color.Red * 0.4f, 0.4f);
            }
        }

        private void HandleProjectileHits(int index, Projectile node)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (!proj.active || proj.type == ModContent.ProjectileType<HeartNode>() || proj.hostile) continue;

                Rectangle hitbox = proj.Hitbox;
                float collisionPoint = 0f;
                bool hit = Collision.CheckAABBvLineCollision(
                    hitbox.TopLeft(), hitbox.Size(),
                    Projectile.Center, node.Center,
                    4f, ref collisionPoint
                );

                if (hit)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        Spark Spark = new Spark();

                        Spark.PrepareSpark(proj.Center, new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)), 0f, Color.Red, 0.4f, true, 40, SparkDrawMode.Additive);
                        ParticleEngine.BehindProjectiles.Add(Spark);
                    }
                    proj.Kill();
                    SoundEngine.PlaySound(DurabilityDown, proj.Center);
                    connections[index].projDurability--;

                    if (connections[index].projDurability <= 0)
                    {
                        connections[index].active = false;
                    }

                    break;
                }
            }
        }

        public int HitCount = 4;

        private void HandlePlayerHits(int index, Projectile node)
        {
            if (hitCooldown < 40) return;

            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;

                Rectangle hitbox = player.Hitbox;
                float collisionPoint = 0f;
                bool hit = Collision.CheckAABBvLineCollision(
                    hitbox.TopLeft(), hitbox.Size(),
                    Projectile.Center, node.Center,
                    4f, ref collisionPoint
                );

                if (hit)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        Spark Spark = new Spark();

                        Spark.PrepareSpark(player.Center, new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)), 0f, Color.Red, 0.4f, true, 40, SparkDrawMode.Additive);
                        ParticleEngine.BehindProjectiles.Add(Spark);
                    }

                    player.Hurt(new Player.HurtInfo
                    {
                        Damage = Projectile.damage,
                        DamageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{player.name} got caught up in unholy wires.")),
                        Knockback = player.noKnockback ? 0f : Projectile.knockBack,
                        HitDirection = 1
                    });
                    player.AddBuff(ModContent.BuffType<BloodHex>(), 360);

                    HitCount--;
                    if (HitCount <= 0)
                    {
                        player.AddBuff(ModContent.BuffType<MobilityHex>(), 360);
                        HitCount = 4;
                    }

                    SoundEngine.PlaySound(DurabilityDown, player.Center);
                    connections[index].npcDurability--;
                    if (connections[index].npcDurability <= 0)
                    {
                        connections[index].active = false;
                    }

                    hitCooldown = 0;
                    break;
                }
            }
        }

        private void BeginDeathCountdown()
        {
            if (dying) return;

            dying = true;
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 60);

            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(40, 80);
              
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(kill, Projectile.Center);
            if (Projectile.timeLeft <= 1)
            {
                //PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Projectile.Center, Vector2.Zero, Color.Red, 1f);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<PrimalBlood>(), Main.rand.Next(6, 9), Projectile.Center, Projectile.damage, 4, 7, ai0: 0, ai1: 0.03f, offset: 0f);
            }
        }
    }
}
