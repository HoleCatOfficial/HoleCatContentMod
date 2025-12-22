using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.CursedFlame
{
	public class CursedFlameNapalm : ModProjectile
	{
		public override string Texture => "DestroyerTest/Content/Particles/ParticleDrawEntity";

		private List<Vector2> trailPositions = new List<Vector2>();
		private const int TrailCacheLength = 40;

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.alpha = 160;
		}

		public bool DrawTrail = true;
		private bool FadingTrail = false;
		private float trailFadeSpeed = 0.5f; // how fast the trail collapses


		public override void AI()
		{
			// Record position only if not fading
			if (DrawTrail && !FadingTrail)
			{
				trailPositions.Insert(0, Projectile.Center);
				if (trailPositions.Count > TrailCacheLength)
					trailPositions.RemoveAt(trailPositions.Count - 1);
			}
			else if (FadingTrail)
			{
				// Gradually remove old points to collapse the trail
				if (trailPositions.Count > 0)
				{
					// You can tune how many points are removed per frame
					int collapseCount = (int)Math.Ceiling(trailFadeSpeed);
					for (int i = 0; i < collapseCount && trailPositions.Count > 0; i++)
						trailPositions.RemoveAt(trailPositions.Count - 1);
				}
			}

			// Gravity + motion
			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 5f)
			{
				Projectile.ai[0] = 5f;
				Projectile.velocity.Y += 0.15f;
			}

			if (Projectile.velocity.Y > 16f)
				Projectile.velocity.Y = 16f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (Main.rand.NextBool(3))
			{
				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, new Vector2(0, -0.1f), ColorLib.CursedFlames, 2.5f, 60, ai2: 2);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			var glowTex = DTAssetLib.FeatheredCircle.Value;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			// draw trail first
			for (int i = 0; i < trailPositions.Count; i++)
			{
				float progress = i / (float)TrailCacheLength;
				float scale = MathHelper.Lerp(0.35f, 0.1f, progress);
				Color color = ColorLib.CursedFlames;

				Main.EntitySpriteDraw(
					glowTex,
					trailPositions[i] - Main.screenPosition,
					null,
					color,
					Projectile.rotation,
					glowTex.Size() / 2f,
					scale,
					SpriteEffects.None,
					0
				);
			}

			// draw main projectile glow
			Main.EntitySpriteDraw(
				glowTex,
				Projectile.Center - Main.screenPosition,
				null,
				ColorLib.CursedFlames,
				Projectile.velocity.ToRotation(),
				glowTex.Size() / 2f,
				0.35f,
				SpriteEffects.None,
				0
			);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false; // we handled drawing ourselves
		}

		public bool Flag1 = false;
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = Vector2.Zero;
			DrawTrail = true; // keep drawing while it fades
			FadingTrail = true; // start collapse
			if (!Flag1)
			{
				Projectile.netUpdate = true;
				Flag1 = true;
			}
			return false;
		}


		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.CursedInferno, 240);
		}
	}
}
