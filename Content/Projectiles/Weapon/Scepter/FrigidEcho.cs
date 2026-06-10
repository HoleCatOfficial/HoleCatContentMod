using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class FrigidEcho : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 4;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            //DTUtils.DrawCrystalCore(spriteBatch, Projectile.Center, Color.White, ColorLib.LifeEcho, Projectile.OldCenter().ToList(), R, Projectile, 100, 0.6f);
            return false;
        }

        float R = 0f;

        public override void AI()
        {
            R += 0.01f;

            PointGlowPreMultiplied FX = new();
            FX.Initialize(Projectile.Center, Main.rand.NextVector2Circular(0.5f, 0.5f), ColorLib.LifeEcho, 0.8f);
            ParticleEngine.BehindProjectiles.Add(FX);

            PointGlowPreMultiplied FX2 = new();
            FX2.Initialize(Projectile.Center, Main.rand.NextVector2Circular(0.2f, 0.2f), Color.White, 0.3f);
            ParticleEngine.BehindProjectiles.Add(FX2);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/TenebrisTesticleKill") with { Pitch = 0.8f, PitchVariance = 0.1f }, Projectile.Center);
            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.White, 0.2f, 0.003f, 1.4f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);

            SimpleExplosionParticle Explosion2 = new();
            Explosion2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.LifeEcho, 0.2f, 0.003f, 1.7f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion2);

            
            for (int i = 0; i < 10; i++)
            {
                PointGlowPreMultiplied FX = new();
                Vector2 Vel = Main.rand.NextVector2Circular(4, 4);
                FX.Initialize(Projectile.Center, Vel, ColorLib.LifeEcho, 0.8f);
                ParticleEngine.BehindProjectiles.Add(FX);
            }
        }
    }
}
