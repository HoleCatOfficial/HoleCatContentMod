using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;

namespace DestroyerTest.Content.Projectiles
{
    // This Example shows how to implement a simple homing projectile with animation
    public class SoulOfLight_Projectile : ModProjectile
    {
        // Correct asset path
        public override string Texture => "DestroyerTest/Content/Projectiles/SoulOfLight_Projectile";

        // Store the target NPC using Projectile.ai[0]
        private NPC HomingTarget {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults() {
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
        public void DeathPrep(float Threshold = 600)
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

            if (Main.rand.NextBool(3))
            {}
            Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, new Vector2(0, 0.01f), 0, default, 2f);
            //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, new Vector2(0, 0.01f), Color.Pink, 1f);

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
            if (ExplodesWithPattern)
            {
                //A swirl, spiral, or star of some kind made of dusts.
            }
        }
    }
}