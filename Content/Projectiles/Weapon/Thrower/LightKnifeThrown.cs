using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
{
    public class LightKnifeThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        Vector2 StoredVelocity;
        public override void AI()
        {
            if (StuckInNPC)
            {
                if (stuck == null || !stuck.active || stuck.life < 1)
                {
                    Projectile.velocity = StoredVelocity;
                    StuckInNPC = false;
                }

                WasStuckInNPC = true;
                Projectile.velocity *= Vector2.Zero;
                Projectile.Center = stuck.Center - StoredVelocity * 2f;
                Projectile.gfxOffY = stuck.gfxOffY;


            }
            else
            {
                if (WasStuckInNPC)
                {
                    Projectile.velocity = StoredVelocity * 0.5f;
                }
                if (!StuckInTile)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                else
                {
                    Projectile.velocity *= Vector2.Zero;
                }
            }

            if (Main.rand.NextBool(6) && Projectile.timeLeft > 180)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UndergroundHallowedEnemies, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, 1.2f);
                dust.noGravity = true;
            }

            //Projectile.velocity *= 0.984f;
        }
        public override bool PreDraw(ref Color lightColor)
        {


            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int length = 98;
            float collisionpoint = 0f;
            Vector2 start = Projectile.Center + new Vector2(length * -0.5f, 0).RotatedBy(Projectile.rotation);
            Vector2 end = Projectile.Center + new Vector2(length * 0.5f, 0).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 24, ref collisionpoint);
        }

        bool StuckInNPC = false;
        bool WasStuckInNPC = false;
        NPC stuck = null;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<LightInferno>(), 300);

            if (!StuckInTile)
            {
                if (!StuckInNPC)
                {
                    stuck = target;
                    Projectile.timeLeft = 300;
                    StoredVelocity = Projectile.oldVelocity;
                    SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { MaxInstances = 0, PitchVariance = 0.2f, Pitch = -0.8f }, Projectile.Center);
                    SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);

                    for (int i = 0; i < 10; i++)
                    {
                        Spark Spark = new Spark();
                        Spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(16f, 16f), 0f, ColorLib.SoulOfLightColor, 1f, false, 15, SparkDrawMode.Additive, 3f);
                        ParticleEngine.BehindProjectiles.Add(Spark);
                    }
                    StuckInNPC = true;
                }
                else
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);

                    for (int i = 0; i < 4; i++)
                    {
                        Spark Spark = new Spark();
                        Spark.PrepareSpark(Projectile.Center, (Projectile.rotation + MathHelper.Pi + Main.rand.NextFloat(-0.6f, 0.7f)).ToRotationVector2() * Main.rand.Next(9, 20), Projectile.rotation + MathHelper.PiOver2, ColorLib.SoulOfLightColor, 0.6f, false, 15, SparkDrawMode.Additive, 3f);
                        ParticleEngine.BehindProjectiles.Add(Spark);
                    }

                    Projectile.position += new Vector2(4, 0).RotatedBy(Projectile.rotation);
                }
            }
        }

        bool StuckInTile = false;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!StuckInTile)
            {
                Projectile.timeLeft = 180;
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);
                StuckInTile = true;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);

            if (!StuckInTile && !StuckInNPC)
            {

                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray, Projectile.oldVelocity.X * 4f, Projectile.oldVelocity.Y * 4f, 0, default, 1.2f);
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.15f) * Main.rand.Next(-10, 11);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.UndergroundHallowedEnemies, velocity, 0, default, 1.2f);
                }

                for (int i = 0; i < 9; i++)
                {
                    Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.15f) * Main.rand.Next(-10, 11);
                    Spark spark = new();
                    spark.PrepareSpark(Projectile.Center, velocity, velocity.ToRotation() + MathHelper.PiOver2, ColorLib.SoulOfLightColor, 0.5f, false, 15, SparkDrawMode.Additive, 2f);
                    ParticleEngine.BehindProjectiles.Add(spark);
                }
            }
        }
    }
}