using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class MidnightPollen : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        float GlobalMult = 1f;

        public override void SetDefaults()
        {
            
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            GlobalMult = (float)Main.rand.Next(1, 4);
            Projectile.width = (int)(36 * GlobalMult);
            Projectile.height = (int)(36 * GlobalMult);
        }

        float XScale = 0.2f;
        float YScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.Center - Main.screenPosition, null, Color.Blue with { A = 0 }, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 2f * GlobalMult, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.DarkViolet with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(XScale, 1f) * 0.1f * GlobalMult, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.DarkViolet with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(YScale, 1f) * 0.1f * GlobalMult, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.Plum with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(XScale, 1f) * 0.06f * GlobalMult, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.Plum with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(YScale, 1f) * 0.06f * GlobalMult, SpriteEffects.None);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.6f);

            Projectile.velocity *= 0.95f;

            XScale = Opus.Sine(0.2f, 1f, 0.2f);
            YScale = Opus.Sine(1f, 0.2f, 0.2f);

            
            PixelParticle Particle = new PixelParticle();
            Particle.Initialize(Projectile.Center, Main.rand.NextVector2Circular(0.3f, 0.3f), Color.DarkViolet, 2f);
            ParticleEngine.Particles.Add(Particle);

            TintableSmoke cloud = new();
            cloud.CreateWithBlending(Projectile.Center, Main.rand.NextVector2Circular(1.5f, 1.5f), Color.Blue, 0.2f, 1.2f, 30, PixelLayer.AboveTiles, BlendState.Additive, true);
            ParticleEngine.Particles.Add(cloud);

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.EnergyBounce with { Pitch = 1.2f, PitchVariance = 0.2f, MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/NatureScepterSpray") with { Volume = 0.5f, PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
            Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MidnightBurst>(), Projectile.damage, 12, Projectile.owner, GlobalMult);
        }

    }
}