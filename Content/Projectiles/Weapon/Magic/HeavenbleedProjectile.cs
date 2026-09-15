using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class HeavenbleedProjectile : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
        }

        float Mult = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Mult = Opus.Sine(1f, 1.1f, 0.3f);


            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, Color.PaleGoldenrod with { A = 0 }, MathHelper.PiOver2, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(0.6f, 2f) * Mult, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, Color.PaleGoldenrod with { A = 0 }, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(0.2f, 1f) * Mult, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.ThinGlowCone.Value, Projectile.Center - Main.screenPosition, null, Color.PaleGoldenrod with { A = 0 }, -MathHelper.PiOver2, new Vector2(0f, DTAssetLib.ThinGlowCone.Value.Height / 2), 0.5f * Mult, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                PixelParticle Pixel = new();
                Pixel.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(Main.rand.NextFloat(-0.005f, 0.005f), -2f), Main.rand.NextBool() ? Color.White : Color.PaleGoldenrod, 3f);
                ParticleEngine.Particles.Add(Pixel);
            }

            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * Mult);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new Rectangle((int)Projectile.TopLeft.X, (int)Projectile.TopLeft.Y - 170, 16, 170);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
