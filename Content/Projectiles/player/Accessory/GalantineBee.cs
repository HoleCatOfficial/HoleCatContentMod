using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class GalantineBee : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void AI()
        {
            AnimateProjectile();
            Projectile.spriteDirection = Projectile.direction;

            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width + 2, Projectile.height + 2, DustID.TintableDustLighted, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 150, ColorLib.StellarColor, 0.7f);
            }
            
            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            float maxDetectRadius = 1400f; // The maximum radius at which a projectile can detect a target

            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() * 0.01f;
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
        }
    }
}