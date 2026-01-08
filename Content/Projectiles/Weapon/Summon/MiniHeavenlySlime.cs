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
    public enum HeavenlySlimeState : int
    {
        Idle = 0,
        LockOn = 1,
        Attack = 2
    }

    public class MiniHeavenlySlime : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 0.3f;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }
        private const int RamCooldownTime = 180;
        private const float RamSpeed = 20f;


        public override void AI()
        {
            AnimateProjectile();
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


            HeavenlySlimeState state = (HeavenlySlimeState)(int)Projectile.ai[0];
            int targetIndex = (int)Projectile.ai[1];

            // read this once up-front so we can prefer it anywhere
            int globalTarget = player.MinionAttackTargetNPC;

            if ((state == HeavenlySlimeState.Idle || state == HeavenlySlimeState.LockOn) && Projectile.localAI[1] <= 0)
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

                // If the player has explicitly set a minion attack target, prefer it
                if (globalTarget != -1 && globalTarget != closest)
                {
                    // ensure the global target is actually valid before forcing it
                    if (globalTarget >= 0 && globalTarget < Main.maxNPCs && Main.npc[globalTarget].CanBeChasedBy())
                    {
                        closest = globalTarget;
                    }
                }

                if (closest != -1)
                {
                    Projectile.ai[1] = closest;
                    // ensure the player's MinionAttackTargetNPC reflects our chosen target
                    player.MinionAttackTargetNPC = closest;
                    Projectile.ai[0] = (float)HeavenlySlimeState.Attack;
                    state = HeavenlySlimeState.Attack;
                    Projectile.netUpdate = true; // sync state change
                }
                else
                {
                    // Remain Idle: swarm formation above player
                    DoIdleMovement(player);
                }
            }

            if (state == HeavenlySlimeState.Attack)
            {
                // If the player has set a global target mid-attack, switch to it (if valid)
                if (globalTarget != -1 && globalTarget != targetIndex)
                {
                    if (globalTarget >= 0 && globalTarget < Main.maxNPCs && Main.npc[globalTarget].CanBeChasedBy())
                    {
                        Projectile.ai[1] = globalTarget;
                        targetIndex = globalTarget;
                        Projectile.netUpdate = true; // important for multiplayer sync
                    }
                }

                if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
                {
                    Projectile.ai[0] = (float)HeavenlySlimeState.Idle;
                    Projectile.netUpdate = true;
                    return;
                }

                NPC target = Main.npc[targetIndex];
                if (!target.active || !target.CanBeChasedBy())
                {
                    // lost target, go back to lock-on/search
                    Projectile.ai[0] = (float)HeavenlySlimeState.LockOn;
                    Projectile.ai[1] = -1;
                    Projectile.netUpdate = true;
                    return;
                }

                if (Main.rand.NextBool(3))
                {
                    DoAttackMovement(target, player);
                }
            }
        }


        private void DoIdleMovement(Player player)
        {
            // swarm: compute index among same-owner same-type projectiles to space around the player
            int index = 0;
            int total = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    if (i < Projectile.whoAmI) index++;
                    total++;
                }
            }
            float spacingAngle = MathHelper.TwoPi / Math.Max(1, total);
            float angle = spacingAngle * index + (float)(Main.time / 60.0); // slow shared rotation

            Vector2 idleOffset = new Vector2(0, -60f); // base offset above player
            float radius = 40f + Math.Min(60f, total * 6f);
            Vector2 swirl = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

            Vector2 targetPos = player.Center + idleOffset + swirl;

            // smooth approach
            float inertia = 14f;
            float speed = 12f;
            Vector2 diff = targetPos - Projectile.Center;
            if (diff.Length() > speed)
                diff = Vector2.Normalize(diff) * speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + diff) / inertia;

            Projectile.rotation = Projectile.velocity.ToRotation() * 0.1f;
        }

        public bool B1 = false;

        private void DoAttackMovement(NPC target, Player player)
        {
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

            Projectile.rotation = Projectile.velocity.ToRotation() * 0.1f;

            // "Hit" condition (manual proximity check works better than relying on OnHitNPC for rams)
            if (distance < 4f)
            {
                // Bounce slightly
                Projectile.velocity *= -0.4f;

                // Enter cooldown
                Projectile.localAI[1] = RamCooldownTime;

                Projectile.ai[0] = (float)HeavenlySlimeState.Idle;
                Projectile.ai[1] = -1;

                Projectile.netUpdate = true;
                B1 = false;
            }

            // Safety: target too far
            if (!target.active || !target.CanBeChasedBy())
            {
                Projectile.ai[0] = (float)HeavenlySlimeState.LockOn;
                Projectile.ai[1] = -1;
            }
        }

    }
}