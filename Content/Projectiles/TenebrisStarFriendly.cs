using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class TenebrisStarFriendly : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public float DelayTimer;

        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.TenebrisGradient;
			trailOffset += 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(6).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor * 0.5f, trailOffset, 1);

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(14).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor, trailOffset, 1);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White, true, 0f, 0.9f, 0.9f);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10;
        }

		public override void AI()
		{
			Projectile.ResetExcessTrailPoints();

			DelayTimer++;
			
			Projectile.rotation += Projectile.direction * 0.07f;

			

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

			if (DelayTimer < 20 || DelayTimer > 180)
			{
				return;
			}

			float maxDetectRadius = 2800f;


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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[0];
			if (Projectile.owner > -1)
			{
				player = Main.player[Projectile.owner];
			}

			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
			ShimmeringFlames.ShimmerBurn(target);
			if (player.TryGetModPlayer<TenebrisMagicPlayer>(out var magicPlayer))
			{
				if (magicPlayer.Active)
				{
					player.statMana += (int)(damageDone / 10);
					player.ManaEffect((int)(damageDone / 10));
					for (int u = 0; u < 16; u++)
					{
						Dust.NewDustPerfect(player.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(6, 6), 0, ColorLib.TenebrisGradient);
					}
				}
			}
		}

		

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }

    }

	public class TenebrisStarFriendly_NoHoming : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.TenebrisGradient;
			trailOffset += 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(6).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor * 0.5f, trailOffset, 1);

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(14).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor, trailOffset, 1);

			Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White, true, 0f, 0.9f, 0.9f);

			return false;
		}


	

		public override void AI()
		{
            Projectile.ResetExcessTrailPoints();

            Projectile.rotation += Projectile.direction * 0.07f;

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[0];
			if (Projectile.owner > -1)
			{
				player = Main.player[Projectile.owner];
			}

			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
			ShimmeringFlames.ShimmerBurn(target);
			if (player.TryGetModPlayer<TenebrisMagicPlayer>(out var magicPlayer))
			{
				if (magicPlayer.Active)
				{
					player.statMana += (int)(damageDone / 10);
					player.ManaEffect((int)(damageDone / 10));
					for (int u = 0; u < 16; u++)
					{
						Dust.NewDustPerfect(player.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(6, 6), 0, ColorLib.TenebrisGradient);
					}
				}
			}
		}

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }

    }
}