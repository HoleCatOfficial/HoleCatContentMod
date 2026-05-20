using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class TenebrisBulletProjectile : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public int Variant = Main.rand.Next(3);

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.timeLeft = 360; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            Projectile.extraUpdates = 40;
        }

        public void ColorAffectedFX(Color color)
        {
            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.6f);

            //PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
            //Glow.Initialize(Projectile.Center, Projectile.velocity * 0.3f, color, 0.5f);
            //ParticleEngine.BehindProjectiles.Add(Glow);

            var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, color, 1f);
            d.noGravity = true;

            Spark S = new Spark();

            S.PrepareSpark(Projectile.Center, Projectile.velocity * 0.1f, Projectile.velocity.ToRotation(), color * 0.5f, 1f, false, 20, SparkDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(S);

            /*
            if (Main.rand.NextBool(5))
            {
                PRTLoader.NewParticle(Projectile.Center, new Vector2((Projectile.velocity.X / 2) + Main.rand.NextFloat(-0.5f, 0.5f), (Projectile.velocity.Y / 2) + Main.rand.NextFloat(-0.5f, 0.5f)), PRTLoader.GetParticleID<StarParticle>(), Color.White, 0.5f);
            }
            */
        }

        public override void AI()
        {
            if (Variant == 0)
            {
                ColorAffectedFX(ColorLib.TenebrisBlue);
            }
            if (Variant == 1)
            {
                ColorAffectedFX(ColorLib.TenebrisMagenta);
            }
            if (Variant == 2)
            {
                ColorAffectedFX(ColorLib.TenebrisBeige);
            }

            // Spawn dust along the trail (tweak DustSpawnStep for performance)
            Color color = Variant switch
            {
                0 => ColorLib.TenebrisBlue,
                1 => ColorLib.TenebrisMagenta,
                _ => ColorLib.TenebrisBeige
            };

            
           

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            float maxDetectRadius = 400f;

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
            if (hit.Crit)
            {
                //SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { PitchVariance = 0.2f });
                ShimmeringFlames.ShimmerBurn(target);

                SmallShine shine = new SmallShine();
                shine.Prepare(target.Center, Vector2.Zero, Color.White, 1f);
                ParticleEngine.ShaderParticles.Add(shine);
            }
        }

        public override void OnKill(int timeLeft)
        {
            
        }
	}
}