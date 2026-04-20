using System;
using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems.FractalSummon
{
    public class EternalAbyss : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.75f;
            Projectile.manualDirectionChange = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override bool? CanDamage() => true;

        public static List<int> Blacklist = OpusNPCDropHelper.IgnoreEnemies.ToList();

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HasBuff<LeeFractalBuff>())
            {
                Projectile.timeLeft = 120;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Think(Blacklist);
        }

        //ALL OF Projectile is just decompiled terraria code.

        private void GetMyGroupIndexAndFillBlackList(List<int> blackListedTargets, out int index, out int totalIndexesInGroup) 
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int index1 = 0; index1 < 1000; ++index1)
            {
                Projectile projectile = Main.projectile[index1];
                if (projectile.active && projectile.owner == Projectile.owner && LeeFractal.Swords.Contains(projectile.type) && (projectile.type != ProjectileID.BabyBird || projectile.frame == Main.projFrames[projectile.type] - 1))
                {
                    if (Projectile.whoAmI > index1)
                    {
                        ++index;
                    }
                    ++totalIndexesInGroup;
                }
            }
        }

        private void GetIdlePosition(int stackedIndex, int totalIndexes, out Vector2 idleSpot, out float idleRotation) 
        {
            Player player = Main.player[Projectile.owner];
            int num1 = 1;
            idleRotation = 0.0f;
            idleSpot = Vector2.Zero;
            if (num1 != 0)
            {
                float num2 = (float) (((double) totalIndexes - 1.0) / 2.0);
                idleSpot = player.Center + -Vector2.UnitY.RotatedBy(4.39822959899902 / (double) totalIndexes * ((double) stackedIndex - (double) num2), new Vector2()) * 40f;
                idleRotation = 0.0f;
            }
            int num3 = stackedIndex + 1;
            idleRotation = (float) ((double) num3 * 6.28318548202515 * 0.0166666675359011 * (double) player.direction + 1.57079637050629);
            idleRotation = MathHelper.WrapAngle(idleRotation);
            int num4 = num3 % totalIndexes;
            Vector2 vector2 = new Vector2(0.0f, 0.5f).RotatedBy(((double) player.miscCounterNormalized * (2.0 + (double) num4) + (double) num4 * 0.5 + (double) player.direction * 1.29999995231628) * 6.28318548202515, new Vector2()) * 4f;
            idleSpot = idleRotation.ToRotationVector2() * 10f + player.MountedCenter + new Vector2((float) (player.direction * (num3 * -6 - 16)), player.gravDir * -15f);
            idleSpot += vector2;
            idleRotation += 1.570796f;
        }

        private int TryAttackingNPCs(List<int> blackListedTargets, bool skipBodyCheck = false)
        {
        Vector2 center = Main.player[Projectile.owner].Center;
        int num1 = -1;
        float num2 = -1f;
        for (int index = 0; index < 200; ++index)
        {
            NPC npc = Main.npc[index];
            if (npc.CanBeChasedBy((object) Projectile, false) && (npc.boss || !blackListedTargets.Contains(index)))
            {
            float num3 = npc.Distance(center);
            if ((double) num3 <= 1000.0 && ((double) num3 <= (double) num2 || (double) num2 == -1.0) && (skipBodyCheck || Projectile.CanHitWithOwnBody((Entity) npc)))
            {
                num2 = num3;
                num1 = index;
            }
            }
        }
        return num1;
        }

        private void Think(List<int> blacklist)
        {
            Player player = Main.player[Projectile.owner];
            // Terraprisma timing
            int num1 = 40;
            int num2 = num1 - 1;
            int num3 = num1 + 40;
            int num4 = num3 - 1;
            int num5 = num1 + 1;

            // RETURN TO IDLE
            if (Projectile.ai[0] == -1f)
            {
                GetMyGroupIndexAndFillBlackList(blacklist, out int index, out int total);
                GetIdlePosition(index, total, out Vector2 idleSpot, out float idleRotation);

                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 32f);
                Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.2f);

                if (Projectile.Distance(idleSpot) < 2f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                return;
            }

            // IDLE / TARGET SCANNING
            if (Projectile.ai[0] == 0f)
            {
                GetMyGroupIndexAndFillBlackList(blacklist, out int index, out int total);
                GetIdlePosition(index, total, out Vector2 idleSpot, out float idleRotation);

                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Vector2.SmoothStep(Projectile.Center, idleSpot, 0.45f);
                Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.45f);

                if (Main.rand.Next(20) == 0)
                {
                    int target = TryAttackingNPCs(blacklist, false);
                    if (target != -1)
                    {
                        Projectile.ai[0] = num3;
                        Projectile.ai[1] = target;
                        Projectile.netUpdate = true;
                    }
                }
                return;
            }

            // ATTACK PHASE
            bool skipBodyCheck = true;

            int phase = 0;
            int phaseStart = num2;
            int phaseEnd = 0;

            if (Projectile.ai[0] >= num5)
            {
                phase = 1;
                phaseStart = num4;
                phaseEnd = num5;
            }

            int npcIndex = (int)Projectile.ai[1];

            // Target validation
            if (!Main.npc.IndexInRange(npcIndex) || !Main.npc[npcIndex].CanBeChasedBy(this))
            {
                int newTarget = TryAttackingNPCs(blacklist, skipBodyCheck);
                if (newTarget != -1)
                {
                    Projectile.ai[0] = num3;
                    Projectile.ai[1] = newTarget;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                }
                Projectile.netUpdate = true;
                return;
            }

            NPC npc = Main.npc[npcIndex];
            Projectile.ai[0]--;

            if (Projectile.ai[0] == phaseStart)
            {
                Projectile.direction = Projectile.Center.X < npc.Center.X ? 1 : -1;

                Projectile.localAI[0] = Projectile.Center.X;
                Projectile.localAI[1] = Projectile.Center.Y;

            }

            float lerp = Utils.GetLerpValue(phaseStart, phaseEnd, Projectile.ai[0], true);

            // ORBITING SLASH
            if (phase == 0)
            {
                Vector2 anchor = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                if (lerp >= 0.5f)
                    anchor = Vector2.Lerp(npc.Center, player.Center, 0.5f);

                Vector2 targetCenter = npc.Center;
                float rot = (targetCenter - anchor).ToRotation();

                float spin = Projectile.direction == 1 ? -MathHelper.Pi : MathHelper.Pi;
                spin += -spin * lerp * 2f;

                Vector2 offset = spin.ToRotationVector2();
                offset.Y *= 0.5f;
                offset.Y *= 0.8f + (float)Math.Sin(Projectile.identity * 2.3f) * 0.2f;
                offset = offset.RotatedBy(rot);

                Vector2 diff = targetCenter - anchor;
                float dist = diff.Length() / 2f;

                Projectile.Center = Vector2.Lerp(anchor, targetCenter, 0.5f) + offset * dist;

                float finalRot = MathHelper.WrapAngle(rot + spin);
                Projectile.rotation = finalRot + MathHelper.PiOver4;
                Projectile.velocity = finalRot.ToRotationVector2() * 10f;
                Projectile.position -= Projectile.velocity;
            }

            // OVERHEAD DIVE
            if (phase == 1)
            {
                Vector2 start = new Vector2(Projectile.localAI[0], Projectile.localAI[1]) + new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, lerp, true) * -100f);

                Vector2 toTarget = npc.Center - start;
                Vector2 offset = toTarget.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(toTarget.Length(), 60f, 150f);

                Vector2 end = npc.Center + offset;

                float l1 = Utils.GetLerpValue(0.4f, 0.6f, lerp, true);
                float l2 = Utils.GetLerpValue(0.6f, 1f, lerp, true);

                Projectile.rotation = Projectile.rotation.AngleTowards(
                    toTarget.SafeNormalize(Vector2.Zero).ToRotation() + MathHelper.PiOver4,
                    0.6283185f);

                Projectile.Center = Vector2.Lerp(start, npc.Center, l1);
                if (l2 > 0f)
                    Projectile.Center = Vector2.Lerp(npc.Center, end, l2);
            }

            // END OF ATTACK
            if (Projectile.ai[0] == phaseEnd)
            {
                int next = TryAttackingNPCs(blacklist, skipBodyCheck);
                if (next != -1)
                {
                    Projectile.ai[0] = num3;
                    Projectile.ai[1] = next;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                }
                Projectile.netUpdate = true;
            }
        }

    }
}