using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
	public class VortexDart : ModProjectile, IDrawPixelated
	{

		public ref float DelayTimer => ref Projectile.ai[1];

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 150;
        }

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		private void AnimateProjectile()
		{
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(0, 242, 170);
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
			return false;
		}

		public override bool? CanHitNPC(NPC target)
		{
			return null;
		}

		public override void AI() 
		{
			Projectile.ResetExcessTrailPoints();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			float maxDetectRadius = 800f; 
			AnimateProjectile();

			if (DelayTimer < 35)
			{
				DelayTimer++;
				return;
			}

			AnimateProjectile();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Electrified, 300);
		}

		void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
		{
			trailOffset += 0.01f;

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, ColorLib.Vortex, trailOffset, 10);
        }
	}
}