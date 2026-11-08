using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Vanilla
{
    public class SwordBeam : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 5f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 900;

        bool IHomingProjectile.CanHome => t > 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SwordBeam);
            Projectile.hide = false;
            Projectile.alpha = 0;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, i / (float)Projectile.oldPos.Length);
                Color color = Color.DarkGoldenrod;
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.3f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.1f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 } * 0.5f) with { scale = new Vector2(Projectile.scale * 1.5f)});
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
            return false;
        }
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            PixelParticle pixel = new();
            pixel.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.2f, Color.Goldenrod, 2f);
            ParticleEngine.Particles.Add(pixel);
        }

        int t = 0;
        public override void PostAI()
        {
            t++;
        }
    }
}
