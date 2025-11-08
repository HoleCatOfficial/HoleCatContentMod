using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
{
    public class ConstitutionBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SwordBeam);
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.netImportant = true;
			Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
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
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(SB, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.ConstitutionBeamGlow.Value, Projectile.Center, null, BeamColor * 0.7f, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(SB);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, BeamColor, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}