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
    public enum CrystalSlimeState : int
    {
        Idle = 0,
        LockOn = 1,
        Attack = 2
    }

    public class MiniCrystalSlime : ModProjectile
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

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

            if (Projectile.localAI[1] > 0)
            {
                Projectile.localAI[1]--;
            }

            CrystalSlimeState CrystalSlimeState = (CrystalSlimeState)(int)Projectile.ai[0];
            int targetIndex = (int)Projectile.ai[1];

            int globalTarget = player.MinionAttackTargetNPC;

            if (CrystalSlimeState == CrystalSlimeState.Idle || CrystalSlimeState == CrystalSlimeState.LockOn)
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
                    Projectile.ai[0] = (float)CrystalSlimeState.Attack;
                    CrystalSlimeState = CrystalSlimeState.Attack;
                    Projectile.netUpdate = true;
                }
                else
                {
                    DoIdleMovement(player);
                }
            }

            if (CrystalSlimeState == CrystalSlimeState.Attack)
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
                    Projectile.ai[0] = (float)CrystalSlimeState.Idle;
                    Projectile.netUpdate = true;
                    return;
                }

                NPC target = Main.npc[targetIndex];
                if (!target.active || !target.CanBeChasedBy())
                {
                    // lost target, go back to lock-on/search
                    Projectile.ai[0] = (float)CrystalSlimeState.LockOn;
                    Projectile.ai[1] = -1;
                    Projectile.netUpdate = true;
                    return;
                }

                if (Main.rand.NextBool(3) && Projectile.localAI[1] <= 0)
                {
                    DoAttackMovement(target, player);
                }
            }
        }


        private void DoIdleMovement(Player player)
        {
            int index = 0;
            int total = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && (p.type == Projectile.type || p.type == ModContent.ProjectileType<MiniBouncySlime>()))
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
                Projectile.tileCollide = false;

            }
            else if (player.Distance(Projectile.Center) > 700)
            {
                Projectile.Center = player.Center;
            }
            else
            {
                Projectile.frame = 0;
                Projectile.velocity.Y += 0.6f;
                Projectile.tileCollide = true;
            }

            if (Projectile.Bottom.Y < player.Bottom.Y - 10)
            {
                IgnorePlatform = true;
            }
            else
            {
                IgnorePlatform = false;
            }

            Projectile.rotation = 0;
        }
        private const int RamCooldownTime = 80;
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

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Main.GameUpdateCount % 30 == 0)
            {
                Opus.RadialSpreadProjectile(ProjectileID.CrystalStorm, 4, Projectile.Center, Projectile.damage / 4, 4, 3, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }

            if (distance < 4f)
            {
                for(int r = 0; r < 10; r++)
                {
                    Dust.NewDust(Projectile.Bottom, Projectile.width, 10, DustID.BlueCrystalShard, Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-0.75f, 0.75f), 0, default, 1.35f);
                }

                // Bounce slightly
                Projectile.velocity *= -0.4f;

                // Enter cooldown
                Projectile.localAI[1] = RamCooldownTime;

                NPC.HitInfo hit = new NPC.HitInfo()
                {
                    Damage = Projectile.damage,
                    Knockback = 9,
                    DamageType = DamageClass.Summon,
                    HitDirection = Projectile.direction
                };

                target.StrikeNPC(hit, false, false);

                Projectile.ai[0] = (float)CrystalSlimeState.Idle;
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
                Projectile.ai[0] = (float)CrystalSlimeState.LockOn;
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
                    Dust.NewDust(Projectile.Bottom, Projectile.width, 10, DustID.BlueCrystalShard, Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-0.75f, 0.75f), 0, default, 1.35f);
                }
            }
            return false;
        }
    }
}