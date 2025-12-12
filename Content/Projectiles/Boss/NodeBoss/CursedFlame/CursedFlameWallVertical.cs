using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame
{
	public class CursedFlameWallVertical : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 3000; // The height of projectile hitbox
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            DTUtils Utility = new DTUtils();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.CursedFlames,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2),
                1f,
                SpriteEffects.None,
                0
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
            return false;
        }

		public override void AI()
        {
            for (int v = 0; v < 20; v++)
            {
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero, ColorLib.CursedFlames, 1.5f, 60, ai2: 2);
            }
        }
	}
}