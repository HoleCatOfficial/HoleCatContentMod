using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
	public class SpookySickleExplosion : ModProjectile
	{
        private void AnimateProjectile() {
            if (++Projectile.frameCounter >= 3) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            var texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle source = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Vector2 origin = source.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition;

            Color customColor = new Color(252, 121, 2) * Projectile.Opacity;
            Color drawColor = Projectile.GetAlpha(customColor);

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(texture, position, source, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false; // skip default draw
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void AI() {
            AnimateProjectile();
            for (int f = 0; f < 25; f++)
            {
                Vector2 Outer = Projectile.Center + Main.rand.NextVector2Circular(800, 800);
                Vector2 Dir = (Projectile.Center - Outer) * 0.005f;
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Dir, 150, default, 3);
            }
			if (Projectile.frame == 4)
            {
                Projectile.Kill();
            }
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
	}
}