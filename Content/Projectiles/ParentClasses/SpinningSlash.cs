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
    public abstract class SpinningSlash : ModProjectile
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
                    Projectile.scale * 1.15f,
                    SpriteEffects.None,
                    0
                );

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor * 0.35f,
                    Projectile.rotation + 0.5f,
                    origin,
                    Projectile.scale * 1.05f,
                    SpriteEffects.None,
                    0
                );

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor * 0.35f,
                    Projectile.rotation - 0.5f,
                    origin,
                    Projectile.scale * 1.05f,
                    SpriteEffects.None,
                    0
                );

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor,
                    Projectile.rotation,
                    origin,
                    Projectile.scale * 0.35f,
                    SpriteEffects.None,
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
                    SpriteEffects.None,
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
                    SpriteEffects.None,
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
                Projectile.alpha += (int)(255f / 40f);;
            }

            Rotation();
            ExtraEffects();

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 rand = Projectile.Center + Main.rand.NextVector2Circular(Projectile.Hitbox.Width / 2, Projectile.Hitbox.Height / 2);

                    Vector2 outward = Vector2.Normalize(rand - Projectile.Center);

                    Vector2 spiralDir = outward.RotatedBy(Projectile.rotation);

                    Vector2 velocity = spiralDir * DustVelocityMultiplier;

                    if (DustUsesColor)
                    {
                        Dust.NewDustPerfect(rand, DustType, velocity, 0, themeColor, DustScale);
                    }
                    else
                    {
                        Dust.NewDustPerfect(rand, DustType, velocity, 0, default, DustScale);
                    }
                }
            }

            for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2) {
				Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
				Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
			}
        }

        public virtual void Rotation()
        {
            Projectile.rotation += 0.8f * Projectile.direction;
        }
    }
}