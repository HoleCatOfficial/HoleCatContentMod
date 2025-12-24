using System;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{
    public abstract class LinearSlash : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetStaticDefaults() {
			Main.projFrames[Type] = 4;
		}
        public override void SetDefaults()
        {
            Projectile.width = 170;
            Projectile.height = 170;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public Color themeColor;
        public int DustType = DustID.FireworksRGB;
        public bool DustUsesColor = true;
        public float DustScale = 0.5f;
        public bool Blending = true;


		public override bool PreDraw(ref Color lightColor)
        {
            lightColor = themeColor;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);
            SpriteEffects fx = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Blending)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor * 0.5f,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 1.05f,
                    fx,
                    0
                );

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor * 0.35f,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 1.25f,
                    fx,
                    0
                );

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    fx,
                    0
                );

                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
            else
            {
                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    fx,
                    0
                );
            }

            return false;
        }

        private void AnimateProjectile() {
            if (++Projectile.frameCounter >= 60) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public virtual void ExtraEffects()
        {
            
        }

        public float DustVelocityMultiplier = 2f;
        public override void AI()
        {
            AnimateProjectile();
            if (Projectile.timeLeft > 80)
            {
                Projectile.alpha -= (int)(255f / 40f);
            }
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += (int)(255f / 40f);
            }

            ExtraEffects();

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 8; i++)
                {
                    
                    if (DustUsesColor)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, themeColor, DustScale);
                    }
                    else
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, DustScale);
                    }
                }
            }

            for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2) {
				Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
				Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
			}

            if (TileDeath)
            {
                Projectile.scale *= 0.99f;
                Projectile.velocity *= 0.001f;
            }

            Rectangle tileHitbox = TileCollideHitbox();
            if (!TileDeath && Collision.SolidCollision(tileHitbox.TopLeft(), tileHitbox.Width, tileHitbox.Height))
            {
                TileDeathProtocol();
            }
        }

        public Rectangle TileCollideHitbox()
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32));
        }

        public bool TileDeath = false;
        public void TileDeathProtocol()
        {
            Projectile.timeLeft = 40;
            TileDeath = true;
        }
    }
}