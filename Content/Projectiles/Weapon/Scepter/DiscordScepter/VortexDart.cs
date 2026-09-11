using System.Collections.Generic;
using System.IO;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
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

		public float DelayTimer;

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
			if (++Projectile.frameCounter >= 5)
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

        public void DustSpawn1()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, -20).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, 20).RotatedBy(Projectile.rotation + MathHelper.PiOver2);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

			Spark spark1 = new();
			spark1.PrepareSpark(DustPos, -Projectile.velocity * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.Vortex, 0.4f, false, 30, SparkDrawMode.Additive, 2.6f);
			ParticleEngine.Particles.Add(spark1);

            Vector2 Pos3 = Projectile.Center + new Vector2(0, 20).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
            Vector2 Pos4 = Projectile.Center + new Vector2(0, -20).RotatedBy(Projectile.rotation + MathHelper.PiOver2);

            Vector2 DustPos2 = Opus.Sine(Pos3, Pos4, 0.5f);

            Spark spark2 = new();
            spark2.PrepareSpark(DustPos2, -Projectile.velocity * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.Vortex, 0.4f, false, 30, SparkDrawMode.Additive, 2.6f);
            ParticleEngine.Particles.Add(spark2);
        }

        public override void AI() 
		{
            DustSpawn1();

            Projectile.ResetExcessTrailPoints();
			Projectile.rotation = Projectile.velocity.ToRotation();
			AnimateProjectile();

            Lighting.AddLight(Projectile.Center, ColorLib.Vortex.ToVector3());

           
			if (DelayTimer < 35)
			{
				DelayTimer++;
				return;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.ModItem is CelestialDiscord CD && CD.Charge < 100)
            {
                CD.Charge++;
                CD.ChargeIncrementInterval = 60;
            }

            target.AddBuff(BuffID.Electrified, 300);
		}

		void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
		{
			trailOffset += 0.01f;

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 7, ColorLib.Vortex with { A = 0 }, trailOffset, 10);

			spriteBatch.ResetToDefault();
		}
	}
}