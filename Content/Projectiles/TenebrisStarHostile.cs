using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
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
using OpusLib;
using DestroyerTest.Content.Equips;
using System.Linq;

namespace DestroyerTest.Content.Projectiles
{
	public class TenebrisStarHostile : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;

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
			ProjectileID.Sets.TrailCacheLength[Type] = 160;
			ProjectileID.Sets.TrailingMode[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = true;
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

        public override bool CanHitPlayer(Player target)
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

		

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
          
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 30 * 60);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
			}
		}

    }

    public class TenebrisStarHostile_NoHoming : ModProjectile
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
			Projectile.friendly = false;
			Projectile.hostile = true;
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

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
          
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 30 * 60);
		}

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            }
        }

    }
}