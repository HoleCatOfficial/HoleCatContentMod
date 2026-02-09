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
    public enum StarState : int
    {
        Idle = 0,
        LockOn = 1,
        Attack = 2
    }

    public class StarConstructMinion : ModProjectile
    {
        // timers go into localAI to persist across networking, ai[0] = state, ai[1] = target NPC index (when attacking)
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile,  ColorLib.StellarFireGradientLooping(3f), false);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping(3f), Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.Sparkle(5).Value, Projectile.Center - Main.screenPosition, null, Color.GhostWhite, 0f, DTAssetLib.Sparkle(5).Value.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<StarConstructMinionBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<StarConstructMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            StarState state = (StarState)(int)Projectile.ai[0];
            int targetIndex = (int)Projectile.ai[1];

            // read this once up-front so we can prefer it anywhere
            int globalTarget = player.MinionAttackTargetNPC;

            if (state == StarState.Idle || state == StarState.LockOn)
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
                    Projectile.ai[0] = (float)StarState.Attack;
                    state = StarState.Attack;
                    Projectile.netUpdate = true; // sync state change
                }
                else
                {
                    // Remain Idle: swarm formation above player
                    DoIdleMovement(player);
                }
            }

            if (state == StarState.Attack)
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
                    Projectile.ai[0] = (float)StarState.Idle;
                    Projectile.netUpdate = true;
                    return;
                }

                NPC target = Main.npc[targetIndex];
                if (!target.active || !target.CanBeChasedBy())
                {
                    // lost target, go back to lock-on/search
                    Projectile.ai[0] = (float)StarState.LockOn;
                    Projectile.ai[1] = -1;
                    Projectile.netUpdate = true;
                    return;
                }

                DoAttackMovement(target, player);
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

        private void DoAttackMovement(NPC target, Player player)
        {
            // orbit target
            Projectile.localAI[0]++; // used as attack timer
            float orbitRadius = 100f;
            float orbitSpeed = 0.06f; // radians per tick
            int idx = 0;
            int total = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    if (i < Projectile.whoAmI) idx++;
                    total++;
                }
            }
            float baseAngle = (float)(Main.time * orbitSpeed) + (MathHelper.TwoPi * idx / Math.Max(1, total));
            Vector2 orbitPos = target.Center + new Vector2((float)Math.Cos(baseAngle), (float)Math.Sin(baseAngle)) * orbitRadius;

            // smooth move toward orbitPos
            float inertia = 8f;
            float speed = 16f;
            Vector2 desired = orbitPos - Projectile.Center;
            if (desired.Length() > speed)
                desired = Vector2.Normalize(desired) * speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + desired) / inertia;

            // firing logic
            int fireRate = 100; // ticks between shots
            if ((int)Projectile.localAI[0] % fireRate == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    SoundEngine.PlaySound(SoundID.Item125 with { MaxInstances = 0 }, Projectile.Center);
                    Vector2 shootVel = Vector2.Normalize(target.Center - Projectile.Center) * 10f;
                    // spawn a small summon projectile (use Stardust cell-like projectile as an example)
                    int projType = ModContent.ProjectileType<MiniCometFriendly>(); // replace or create custom projectile type as desired
                    int newProj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, projType, Projectile.damage, 0f, Projectile.owner, ai2: 1);
                    Main.projectile[newProj].DamageType = DamageClass.Summon;
                }
            }

            // if target gets too far, drop back to lock-on search
            if (Vector2.DistanceSquared(Projectile.Center, target.Center) > 2500f * 2500f)
            {
                Projectile.ai[0] = (float)StarState.LockOn;
                Projectile.ai[1] = -1;
            }
        }
    }
}