using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using System;

namespace DestroyerTest.Content.Projectiles
{
    public class SoulOfNight_Projectile : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/SoulOfNight_Projectile";

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
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[Projectile.type] = 4; // Set the number of frames in the sprite sheet
        }

        public override void SetDefaults()
        {
            Projectile.width = 22; // The width of projectile hitbox
            Projectile.height = 22; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Melee; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.frame = 0; // Start at the first frame
        }

        public bool ExplodesWithPattern = false;
        public void DeathPrep(float Threshold = 60)
        {
            if (Projectile.timeLeft > Threshold)
            {
                return;
            }

            if (Projectile.timeLeft <= Threshold)
            {
                if (Projectile.velocity.Length() > 0.01f)
                {
                    Projectile.velocity *= 0.999f;
                }

                if (Projectile.timeLeft < 10)
                {
                    ExplodesWithPattern = true;
                }
            }
        }

        public override void AI()
        {
            AnimateProjectile();
            DeathPrep();

            Dust Trail = Dust.NewDustPerfect(Projectile.Center, DustID.DemonTorch, Vector2.Zero, 0, default, 2f);
            Trail.noGravity = true;
            //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, new Vector2(0, 0.01f), Color.Purple, 1f);

            float maxDetectRadius = 1600f;

            if (DelayTimer < 10)
            {
                DelayTimer++;
                return;
            }

            // acquire or validate target
            if (HomingTarget == null || !IsValidTarget(HomingTarget))
                HomingTarget = FindClosestNPC(maxDetectRadius);

            if (HomingTarget == null)
                return; // no target? just hover

            // we have a valid target at this point
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            float speed = Projectile.velocity.Length();

            if (speed < 0.01f)
            {
                // give it an initial push toward the target
                float startSpeed = 6f;   // pick whatever feels right
                Projectile.velocity = targetAngle.ToRotationVector2() * startSpeed;
            }
            else
            {
                // steer current velocity toward target without changing speed
                float turnRate = MathHelper.ToRadians(9);
                Projectile.velocity =
                    Projectile.velocity
                        .ToRotation()
                        .AngleTowards(targetAngle, turnRate)
                        .ToRotationVector2() * speed;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() * 0.05f;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
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
            return target.CanBeChasedBy();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarBurst2") with {MaxInstances = 0});
            if (ExplodesWithPattern)
            {
                Vector2 center = Projectile.Center;

                int points = 300;              // total dusts
                float a = 2.5f;                // base radius factor
                float[] exponents = { 0.5f, 1f, -0.4f }; // Fermat, Archimedean, inward spiral
                float[] ks = { 0.15f, -0.2f }; // for logarithmic r = a e^{kφ}

                for (int i = 0; i < points; i++)
                {
                    // pick a spiral type at random
                    int style = Main.rand.Next(4);
                    float φ = i * 0.1f + Main.rand.NextFloat(0f, 0.3f); // add jitter
                    float r = 0f;

                    switch (style)
                    {
                        case 0: // power spiral
                            float n = exponents[Main.rand.Next(exponents.Length)];
                            r = a * (float)Math.Pow(φ, n);
                            break;
                        case 1: // logarithmic
                            float k = ks[Main.rand.Next(ks.Length)];
                            r = a * (float)Math.Exp(k * φ);
                            break;
                        case 2: // simple Archimedean
                            r = a * φ;
                            break;
                        default: // tight lituus-style
                            r = a / (float)Math.Sqrt(Math.Max(φ, 0.1f));
                            break;
                    }

                    // position on the chosen spiral
                    Vector2 offset = new Vector2(r, 0f).RotatedBy(φ);
                    Vector2 spawnPos = center + offset;

                    // outward velocity with some tangent twist
                    // tangent angle α: tan α = r'/r  (approx here with small delta)
                    float drdφ = (a * (float)Math.Pow(φ + 0.01f, 1) - r) / 0.01f;
                    float alpha = (float)Math.Atan2(drdφ, r);
                    Vector2 vel =
                        offset.SafeNormalize(Vector2.UnitY).RotatedBy(alpha * 0.5f) *
                        Main.rand.NextFloat(2f, 6f);

                    int dustType = Main.rand.NextBool() ? DustID.DemonTorch
                                                        : DustID.PurpleCrystalShard;

                    Dust d = Dust.NewDustPerfect(spawnPos, dustType, vel, 150,
                                                default, Main.rand.NextFloat(1f, 2f));
                    d.noGravity = true;
                    d.fadeIn = Main.rand.NextFloat(0.5f, 1.2f);

                }
            }
        }

    }
}