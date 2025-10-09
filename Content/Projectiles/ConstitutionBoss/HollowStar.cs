using System;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
{
    public class HollowStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * 0.3f;

            if (Main.rand.NextBool(12))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Projectile.velocity * 0.2f, 100, ColorLib.StellarColor, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Color BeamColor = ColorLib.StellarColor;
            lightColor = BeamColor;
            SpriteBatch SB = Main.spriteBatch;
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            DTUtils Utility = new DTUtils();

            Utility.StartSpriteBatchWithBlending(SB, BlendState.Additive, SpriteSortMode.Immediate);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, BeamColor, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            Utility.ReturnToDefaultDrawing(SB);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            DTUtils.ConstitutionStarExplosionEffects(Projectile);
        }
    }
}