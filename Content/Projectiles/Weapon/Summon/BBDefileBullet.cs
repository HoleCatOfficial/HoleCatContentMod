using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class BBDefileBullet : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        float Scl = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            
            //Opus.DrawTextureOnProj(DTAssetLib.CorruptSigil, Projectile, OpusColorUtils.MultiLerp(prog, ColorLib.WretchedColorMap) with { A = 0 }, false, 0f, Scl * 0.5f, Scl * 0.5f);
            //Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5, true), Projectile, Color.White with { A = 0 }, false, 0f, Scl, Scl);

            return false;
        }

        public override void AI()
        {
            Scl = Opus.Sine(0.8f, 0.4f, 0.6f);

            float prog = ((float)Projectile.timeLeft / 1200f).Inverse();

            //var G = new PointGlowPreMultiplied();
            //G.Initialize(Projectile.Center, Vector2.Zero, ColorLib.Wretched5 * 0.25f, 1f);
            //G.MaxLifetime = 60;
            //ParticleEngine.BehindProjectiles.Add(G);

            var P = DamnationParticle.Create(Projectile.Center, Projectile.velocity * 0.1f, Main.rand.NextFloat(0.04f, 0.8f), 60, PixelLayer.AbovePlayer);
            ParticleEngine.ShaderParticles.Add(P);

    
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item20, target.Center);
            target.AddBuff(ModContent.BuffType<Defilement>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            
        }
    }
}