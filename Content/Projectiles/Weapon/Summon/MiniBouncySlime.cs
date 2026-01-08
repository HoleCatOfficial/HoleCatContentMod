using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public enum BouncySlimeState : int
    {
        Idle = 0,
        LockOn = 1,
        Attack = 2
    }

    public class MiniBouncySlime : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 0.3f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<HallowedSlimesBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<HallowedSlimesBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            BouncySlimeState BouncySlimeState = (BouncySlimeState)(int)Projectile.ai[0];
            int targetIndex = (int)Projectile.ai[1];

            int globalTarget = player.MinionAttackTargetNPC;

            if (BouncySlimeState == BouncySlimeState.Idle || BouncySlimeState == BouncySlimeState.LockOn)
            {
                int closest = -1;
                float closestDist = 2000f * 2000f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float dist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closest = i;
                        }
                    }
                }

                if (globalTarget != -1 && globalTarget != closest)
                {
                    if (globalTarget >= 0 && globalTarget < Main.maxNPCs && Main.npc[globalTarget].CanBeChasedBy())
                    {
                        closest = globalTarget;
                    }
                }

                if (closest != -1)
                {
                    Projectile.ai[1] = closest;
                    player.MinionAttackTargetNPC = closest;
                    Projectile.ai[0] = (float)BouncySlimeState.Attack;
                    BouncySlimeState = BouncySlimeState.Attack;
                    Projectile.netUpdate = true;
                }
                else
                {
                    DoIdleMovement(player);
                }
            }

            if (BouncySlimeState == BouncySlimeState.Attack)
            {
                if (globalTarget != -1 && globalTarget != targetIndex)
                {
                    if (globalTarget >= 0 && globalTarget < Main.maxNPCs && Main.npc[globalTarget].CanBeChasedBy())
                    {
                        Projectile.ai[1] = globalTarget;
                        targetIndex = globalTarget;
                        Projectile.netUpdate = true;
                    }
                }

                if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
                {
                    Projectile.ai[0] = (float)BouncySlimeState.Idle;
                    Projectile.netUpdate = true;
                    return;
                }

                NPC target = Main.npc[targetIndex];
                if (!target.active || !target.CanBeChasedBy())
                {
                    // lost target, go back to lock-on/search
                    Projectile.ai[0] = (float)BouncySlimeState.LockOn;
                    Projectile.ai[1] = -1;
                    Projectile.netUpdate = true;
                    return;
                }

                DoAttackMovement(target, player);
            }
        }


        private void DoIdleMovement(Player player)
        {
            int index = 0;
            int total = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && (p.type == Projectile.type || p.type == ModContent.ProjectileType<MiniCrystalSlime>()))
                {
                    if (i < Projectile.whoAmI) index++;
                    total++;
                }
            }

            float spacing = 28f;
            float side = (index - (total - 1) / 2f) * spacing;

            Vector2 idlePos = player.Center + new Vector2(side, -20f);
            Vector2 toIdle = idlePos - Projectile.Center;

            if (toIdle.Length() > 10f)
            {
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    toIdle * 0.05f,
                    0.1f
                );
            }
            else
            {
                Projectile.velocity.X *= 0.9f;
            }

            if (player.Distance(Projectile.Center) > 100)
            {
                Projectile.frame = 1;
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    (player.Center - Projectile.Center) * 0.05f,
                    0.1f
                );

            }
            else
            {
                Projectile.frame = 0;
                Projectile.velocity.Y += 0.6f;
            }

            if (Projectile.Bottom.Y < player.Bottom.Y - 16)
            {
                IgnorePlatform = true;
            }
            else
            {
                IgnorePlatform = false;
            }
            
        }

        private const int RamCooldownTime = 180;
        private const float RamSpeed = 20f;
        public bool B1 = false;

        private void DoAttackMovement(NPC target, Player player)
        {
            Projectile.localAI[0]++;
            Vector2 toTarget = target.Center - Projectile.Center;
            float distance = toTarget.Length();

            if (!B1)
            {
                SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
                B1 = true;
            }

            if (distance > 10f)
            {
                Vector2 desiredVelocity = Vector2.Normalize(toTarget) * RamSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.2f);
            }

            Projectile.frame = 1;

            Projectile.rotation = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) * 0.1f;

            if (Main.GameUpdateCount % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity / 2, ModContent.ProjectileType<BouncySlimeBallMini>(), Projectile.damage / 4, 5, Projectile.owner);
            }

            if (distance < 4f)
            {
                for(int r = 0; r < 10; r++)
                {
                    Dust.NewDust(Projectile.Bottom, Projectile.width, 10, DustID.PinkSlime, Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-0.75f, 0.75f), 0, default, 1.35f);
                }
                
                // Bounce slightly
                Projectile.velocity *= -0.4f;

                // Enter cooldown
                Projectile.localAI[1] = RamCooldownTime;

                Projectile.ai[0] = (float)BouncySlimeState.Idle;
                Projectile.ai[1] = -1;

                Projectile.netUpdate = true;
                B1 = false;
            }

            if (Projectile.Bottom.Y < target.Bottom.Y -10)
            {
                IgnorePlatform = true;
            }
            else
            {
                IgnorePlatform = false;
            }

            if (Vector2.DistanceSquared(Projectile.Center, target.Center) > 2500f * 2500f)
            {
                Projectile.ai[0] = (float)BouncySlimeState.LockOn;
                Projectile.ai[1] = -1;
            }
        }

        public bool IgnorePlatform = false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (IgnorePlatform)
            {
                fallThrough = true;
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.frame = 0;
            if (oldVelocity.Y > 5)
            {
                for(int r = 0; r < 10; r++)
                {
                    Dust.NewDust(Projectile.Bottom, Projectile.width, 10, DustID.PinkSlime, Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-0.75f, 0.75f), 0, default, 1.35f);
                }
            }
            return false;
        }
    }
}