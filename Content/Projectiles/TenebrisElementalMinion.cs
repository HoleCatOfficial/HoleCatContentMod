using System;
using System.Linq;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace DestroyerTest.Content.Projectiles
{
    public class TenebrisElementalMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000; // stays alive indefinitely while buff is active
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.minionSlots = 1f;
        }

        public enum State
        {
            OrbitPlayer,
            FlyToNPC,
            OrbitNPC
        };

        public State currentState = State.OrbitPlayer;
        private NPC target;
        private float orbitAngle;
        private const float OrbitRadius = 80f;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!Valid(player))
            {
                Projectile.Kill();
                return;
            }

            AnimateProjectile();

            // Always reset lifetime while valid
            Projectile.timeLeft = 2;

            // Try to acquire a target every so often
            if (target == null || !target.active || !target.CanBeChasedBy(this))
            {
                if (Main.GameUpdateCount % 30 == 0 && player.MinionAttackTargetNPC > -1)
                    target = Main.npc[player.MinionAttackTargetNPC];
                if (Main.GameUpdateCount % 30 == 0 && player.MinionAttackTargetNPC <= -1)
                    target = null;

                if (target != null && target.active)
                    currentState = State.FlyToNPC;
                else
                    currentState = State.OrbitPlayer;
            }

            // State machine
            switch (currentState)
            {
                case State.OrbitPlayer:
                    IdleAI(player);
                    break;

                case State.FlyToNPC:
                    if (target == null || !target.active)
                    {
                        currentState = State.OrbitPlayer;
                        break;
                    }
                    GoToTarget(target);
                    break;

                case State.OrbitNPC:
                    if (target == null || !target.active)
                    {
                        currentState = State.OrbitPlayer;
                        break;
                    }
                    OrbitTarget(target);
                    Attack();
                    break;
            }
        }

        public bool Valid(Player player)
        {
            return player != null && player.active && !player.dead && player.HasBuff<TenebrisCrystalMinionBuff>();
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

        public void IdleAI(Player player)
        {
            // Shared angular reference for all minions (keeps them synchronized)
            float baseAngle = (Main.GameUpdateCount * 0.03f) % MathHelper.TwoPi;

            // Determine this minion's angular offset among siblings
            var sameType = Main.projectile
                .Where(p => p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                .OrderBy(p => p.identity)
                .ToList();

            int index = sameType.IndexOf(Projectile);
            int total = sameType.Count;
            float spacing = MathHelper.TwoPi / Math.Max(1, total);

            float angle = baseAngle + index * spacing;
            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * OrbitRadius;


            Vector2 desiredPosition = player.Center + offset;
            Projectile.velocity = (desiredPosition - Projectile.Center) * 0.1f;

            // Face the direction of orbit travel
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }


        public void GoToTarget(NPC target)
        {
            Vector2 toTarget = target.Center - Projectile.Center;
            float distance = toTarget.Length();

            // Move toward the NPC
            if (distance > OrbitRadius * 1.5f)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget.SafeNormalize(Vector2.Zero) * 12f, 0.1f);
            }
            else
            {
                Projectile.velocity *= 0.9f;
                currentState = State.OrbitNPC;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void OrbitTarget(NPC target)
        {

            // Shared angular reference for all minions (keeps them synchronized)
            float baseAngle = (Main.GameUpdateCount * 0.03f) % MathHelper.TwoPi;

            // Determine this minion's angular offset among siblings
            var sameType = Main.projectile
                .Where(p => p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                .OrderBy(p => p.identity)
                .ToList();

            int index = sameType.IndexOf(Projectile);
            int total = sameType.Count;
            float spacing = MathHelper.TwoPi / Math.Max(1, total);

            float angle = baseAngle + index * spacing;
            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * OrbitRadius;


            Vector2 desiredPosition = target.Center + offset;
            Projectile.velocity = (desiredPosition - Projectile.Center) * 0.15f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            
        }

        public void Attack()
        {
            if (Main.GameUpdateCount % 120 == 0)
            {

                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { PitchVariance = 1f, Volume = 3f });

                int numVectors = Main.rand.Next(2, 7);
                float angleStep = MathHelper.TwoPi / numVectors;
                float baseAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float RotSpeed = Main.rand.NextFloat(-16f, -8f);

                for (int i = 0; i < numVectors; i++)
                {
                    float angle = baseAngle + i * angleStep + Main.rand.NextFloat(-0.4f, 0.4f);
                    Vector2 startPos = Projectile.Center + 22f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    Vector2 finalVel = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * RotSpeed;

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        startPos,
                        finalVel,
                        ModContent.ProjectileType<TenebrisFlames>(),
                        Projectile.damage / numVectors,
                        5,
                        Projectile.owner,
                        ai2: 1
                    );
                }
            }
        }
    }
}
