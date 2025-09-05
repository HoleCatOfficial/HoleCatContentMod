using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	/// <summary>
	/// Expermiental Class for an All-Purpose Homing Projectile.
	/// <para/> Projectile ai slots 0 and 1 should not be set to anything when spawning, as they store NPC and Player values respectively.
	/// <para/> Projectile ai slot 2 controls whether the projectile is friendly or harmful.
	/// </summary>
	public class TenebrisStar : ModProjectile
	{
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		private Player PLRTarget
		{
			get => Projectile.ai[1] == 0 ? null : Main.player[(int)Projectile.ai[1] - 1];
			set
			{
				Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Asset<Texture2D> projectileTexture = TextureAssets.Projectile[Projectile.type];
			Asset<Texture2D> pixel = TextureAssets.MagicPixel;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			for (int i = 0; i < TrailPositions.Count - 1; i++)
			{
				Vector2 start = TrailPositions[i] - Main.screenPosition;
				Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
				Vector2 diff = end - start;

				float length = diff.Length();
				if (length < 0.5f)
					continue;

				float rotation = diff.ToRotation();
				float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
				float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);

				// --- Tenebris gradient with offset ---
				float time = (Main.GlobalTimeWrappedHourly + i * 0.05f) % 3f;

				Color tenebrisColor;
				if (time < 1f)
					tenebrisColor = Color.Lerp(ColorLib.TenebrisBeige, ColorLib.TenebrisMagenta, time);
				else if (time < 2f)
					tenebrisColor = Color.Lerp(ColorLib.TenebrisMagenta, ColorLib.TenebrisBlue, time - 1f);
				else
					tenebrisColor = Color.Lerp(ColorLib.TenebrisBlue, ColorLib.TenebrisBeige, time - 2f);

				tenebrisColor *= alpha;

				Main.spriteBatch.Draw(
					pixel.Value,
					start,
					null,
					tenebrisColor,
					rotation,
					new Vector2(pixel.Value.Width / 2, pixel.Value.Height / 2),
					new Vector2(length, width),
					SpriteEffects.None,
					0f
				);
			}


			Main.EntitySpriteDraw(
				projectileTexture.Value,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale * 2,
				SpriteEffects.None,
				0
			);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}


		/// <summary>
		/// Controls whether the Projectile is Hostile or Friendly.
		/// <para/> 1 = Friendly, 2 = Hostile
		/// <para/> Attempting to return an invalid value will kill the projectile.
		/// </summary>
		public int Mode;

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;

		public override void AI()
		{
			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			DelayTimer++;
			Mode = (int)Projectile.ai[2];

			if (Mode > 4 || Mode <= 0)
			{
				Projectile.Kill();
				//throw new Exception("Non-Fatal Error in Oil Projectile Targeting. Value must be 1 or 2.");
				Mod.Logger.Warn("OilProjectile: Invalid Mode in ai[2]. Expected 1 or 2.");
			}

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

			if (DelayTimer < 10)
			{
				DelayTimer += 1;
				return;
			}

			float maxDetectRadius = 1400f;

			if (Mode == 1)
			{
				Projectile.friendly = true;
				Projectile.hostile = false;

				if (NPCTarget == null)
				{
					NPCTarget = FindClosestNPC(maxDetectRadius);
				}


				if (NPCTarget != null && !IsValidNPC(NPCTarget))
				{
					NPCTarget = null;
				}


				if (NPCTarget == null)
					return;

				float length = Projectile.velocity.Length();
				float targetAngle = Projectile.AngleTo(NPCTarget.Center);
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
			}
			if (Mode == 2)
			{
				Projectile.friendly = false;
				Projectile.hostile = true;

				if (PLRTarget == null)
				{
					PLRTarget = FindClosestPlayer(maxDetectRadius);
				}


				if (PLRTarget != null && !IsValidPlayer(PLRTarget))
				{
					PLRTarget = null;
				}

				if (PLRTarget == null)
					return;

				float length = Projectile.velocity.Length();
				float targetAngle = Projectile.AngleTo(PLRTarget.Center);
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * length;
			}
			if (Mode == 3)
			{
				Projectile.friendly = true;
				Projectile.hostile = false;
			}
			if (Mode == 4)
			{
				Projectile.friendly = false;
				Projectile.hostile = true;
			}
		}
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.ActiveNPCs)
			{
				if (IsValidNPC(target))
				{

					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidNPC(NPC target)
		{
			return target.CanBeChasedBy();
		}

		public Player FindClosestPlayer(float maxDetectDistance)
		{
			Player closestPlayer = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			foreach (var target in Main.player)
			{
				if (IsValidPlayer(target))
				{
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestPlayer = target;
					}
				}
			}

			return closestPlayer;
		}

		public bool IsValidPlayer(Player target)
		{
			return target.active == true && target.statLife > 1;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Mode == 1 || Mode == 1)
			{
				PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom1>(), target.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.05f);
				target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 300);
			}

		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Mode == 2 || Mode == 4)
			{
				PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom1>(), target.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.05f);
				target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 300);
			}
		}

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.Center, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }

    }
}