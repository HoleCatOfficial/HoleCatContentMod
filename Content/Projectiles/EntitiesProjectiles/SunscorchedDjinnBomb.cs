using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Projectiles.EntitiesProjectiles
{
    public class SunscorchedDjinnBomb : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        float ROff = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            ROff -= 0.01f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawCrystalCore(spriteBatch, Projectile.Center, Color.Black, ColorLib.Rift, ROff, 1f);
            return false;
        }

        void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center, Color colorIN, Color colorOUT, float TextureRotationOffset, float Scale = 1f)
        {
            DTUtils Utility = new DTUtils();
            float OuterScale = Scale * 0.14f;

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                colorOUT with { A = 0 },
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                OuterScale,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                colorIN,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                Scale,
                SpriteEffects.None,
                1f
            );
        }

        public override void AI()
        {
            Projectile.velocity *= 0.99f;
            if (Projectile.timeLeft == 1)
            {
                Projectile.Resize(200, 200);
            }
        }

        public SoundStyle Burst = DTAssetLib.Impacts.HeatseekerSilohSlam with { PitchVariance = 0.5f };

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Burst);

            

            ImpactCracks Cracks = new();
            Cracks.Prepare(Projectile.Center, ColorLib.LightRift3, 1f);
            ParticleEngine.BehindProjectiles.Add(Cracks);

            //Beeg boy
            SimpleExplosionParticle Burst1 = new SimpleExplosionParticle();
            Burst1.Prepare(Projectile.Center, Vector2.Zero, ColorLib.DarkRift3, 0.1f, 0.01f, 3.2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst1);

            SimpleExplosionParticle Burst2 = new SimpleExplosionParticle();
            Burst2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.1f, 0.01f, 2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst2);

            for (int d = 0; d < 24; d++)
            {
                Vector2 ran = Main.rand.NextVector2Circular(9f, 9f);

                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, ran, (int)MathHelper.Lerp(255, 0, Main.rand.NextFloat(0.5f, 1f)), ColorLib.LightRift3, Main.rand.NextFloat(0.7f, 1.4f));
            }

            for (int d = 0; d < 8; d++)
            {
                Vector2 ran = Main.rand.NextVector2Circular(9f, 9f);

                HeatseekerSilohSpark Spark = new();
                Spark.PrepareSpark(Projectile.Center, ran, ran.ToRotation() + MathHelper.PiOver2, ColorLib.Rift, 1f, false, 80, SparkDrawMode.Additive, 3f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            //Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<RiftStarFriendly>(), 4, Projectile.Center, (int)Owner.GetTotalDamage(DamageClass.Generic).ApplyTo(20), 4, 8f);
            //Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<RiftSpark>(), 6, Projectile.Center, (int)Owner.GetTotalDamage(DamageClass.Generic).ApplyTo(10), 4, 4f);

        }
    }
}
