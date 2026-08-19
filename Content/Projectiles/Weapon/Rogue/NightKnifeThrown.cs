using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Terraria.Audio;
using DestroyerTest.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class NightKnifeThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(6) && Projectile.timeLeft > 180)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, 1.2f);
                dust.noGravity = true;
            }

            Projectile.velocity *= 0.984f;
        }
        public override bool PreDraw(ref Color lightColor)
        {


            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<NightInferno>(), 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { MaxInstances = 0, PitchVariance = 0.2f, Pitch = -0.8f }, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(8f, 8f), 0f, ColorLib.SoulOfNightColor, 0.6f, false, 15, SparkDrawMode.Additive, 2f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray, Projectile.oldVelocity.X * 4f, Projectile.oldVelocity.Y * 4f, 0, default, 1.2f);
            }
        }
    }
}