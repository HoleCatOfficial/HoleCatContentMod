using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class TenebrisFlamesFriendly : ModProjectile
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

		public float DelayTimer = 0;
		public float HomeTimer = 0;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.1f;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		public override bool? CanHitNPC(NPC target)
		{
			return DelayTimer >= 20;
		}
		
		public override void OnSpawn(IEntitySource source)
		{
			
		}

		public int Mode;

		public override void AI()
		{
			DelayTimer++;



			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, ColorLib.TenebrisGradient * 0.75f, 1.8f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);

            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, ColorLib.TenebrisGradient, 1f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire2);

			PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient * 0.5f, 1.25f);

			float maxDetectRadius = 1400f;

			HomeTimer++;
			
			
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

				if (DelayTimer < 60)
				{
					float length = Projectile.velocity.Length();
					float targetAngle = Projectile.AngleTo(NPCTarget.Center);
					Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
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

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ShimmeringFlames.ShimmerBurn(target);
		}

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.Center, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }
    }

	public class TenebrisFlamesFriendly_NoHoming : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.1f;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		
		
		public override void OnSpawn(IEntitySource source)
		{
			
		}

		public int Mode;

		public override void AI()
		{

			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, ColorLib.TenebrisGradient * 0.75f, 1.8f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);

            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, ColorLib.TenebrisGradient, 1f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire2);

            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient * 0.5f, 1.25f);
		}
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ShimmeringFlames.ShimmerBurn(target);
		}

        public override void OnKill(int timeLeft)
        {
			Dust.NewDust(Projectile.Center, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
        }
    }
}