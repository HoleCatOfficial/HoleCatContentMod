using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class HeliciteDart : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
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
        }

        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>(); // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.timeLeft = 360; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            Projectile.extraUpdates = 2;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10;
        }

        private List<Vector2> trailPoints = new List<Vector2>();
        private Vector2 lastTickPosition;
        private const int MaxTrailCount = 60;
        private const int DustSpawnStep = 3; 

        public override void AI()
        {
            int subdivisions = Projectile.extraUpdates + 1;
            Vector2 start = lastTickPosition;
            Vector2 end = Projectile.Center;

            if (start == Vector2.Zero) // safety for first frame
                start = end;

            // Insert interpolated points between last tick and this tick.
            // We append newest at the end, and trim the oldest at index 0 when full.
            for (int s = 1; s <= subdivisions; s++)
            {
                float t = s / (float)subdivisions;
                Vector2 pos = Vector2.Lerp(start, end, t);
                trailPoints.Add(pos);
                if (trailPoints.Count > MaxTrailCount)
                    trailPoints.RemoveAt(0); // drop oldest
            }

            lastTickPosition = end;

            for (int i = 0; i < trailPoints.Count; i += DustSpawnStep)
            {
                Vector2 p = trailPoints[i];
                var d = Dust.NewDustPerfect(p, DustID.TintableDustLighted, Vector2.Zero, 50, ColorLib.Rift, 1f);
                d.noGravity = true;

                //var d2 = Dust.NewDustPerfect(p, DustID.FireworksRGB, Vector2.Zero, 50, ColorLib.Rift, 1f);
                //d2.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3() * 0.6f);

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            float maxDetectRadius = 4000f;

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            int turnspeed = 5;
            turnspeed += 10;
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(turnspeed)).ToRotationVector2() * length;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 300);
        }
	}
}