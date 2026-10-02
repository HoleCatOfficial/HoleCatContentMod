using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class SoulSpit : ModProjectile
    {

        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<WyvernCorpseHead>();
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32; 
            Projectile.friendly = false;
            Projectile.hostile = true; 
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }



        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Opus.DrawTextureOnProj(DTAssetLib.FeatheredCircle, Projectile, ColorLib.Soul2 with { A = 0 }, false, 0f, 1f, 1f);
            Opus.DrawTextureOnProj(DTAssetLib.FeatheredCircle, Projectile, ColorLib.SoulLight with { A = 0 }, false, 0f, 0.6f, 0.6f);
            return false;
        }

        public override void AI()
        {

            PointGlowPreMultiplied glow = new();
            glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.1f, ColorLib.Soul, 0.4f);
            ParticleEngine.Particles.Add(glow);

            PointGlowPreMultiplied glow2 = new();
            glow2.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.1f, ColorLib.Soul3, 0.8f);
            ParticleEngine.Particles.Add(glow2);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            LerpingSimpleExplosionParticle explosion = new();
            explosion.Prepare(Projectile.Center, Vector2.Zero, [ColorLib.Soul, ColorLib.Soul2, ColorLib.Soul3], 0.1f, 0.005f, 1f);
            ParticleEngine.Particles.Add(explosion);

            for (int i = 0; i < 20; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(6f, 6f), 0f, ColorLib.Soul, 0.5f, false, 60, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark);
            }
        }
    }
}