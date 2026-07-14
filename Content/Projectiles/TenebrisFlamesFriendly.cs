using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
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
	public class TenebrisFlamesFriendly : ModProjectile, IHomingProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		public float DelayTimer = 0;
		public float HomeTimer = 0;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 8;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 2800;

        bool IHomingProjectile.CanHome => DelayTimer >= 20 && DelayTimer < 80;



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
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		public override bool? CanHitNPC(NPC target)
		{
			return DelayTimer >= 20 && Projectile.ManualCanHitFriendly(target);
		}
		
		public override void OnSpawn(IEntitySource source)
		{
			
		}

		public int Mode;

		public override void AI()
		{
			DelayTimer++;



			Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

			if (!DTOptimizationsConfig.instance.DisableExcessParticles)
			{
				Fire fire = new Fire();
				fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, ColorLib.TenebrisGradient * 0.5f, 0.8f, 40, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
				ParticleEngine.BehindProjectiles.Add(fire);

                if (Main.rand.NextBool(2))
                {
                    TenebrousCloudParticle Cloud = new();
                    Cloud.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.06f, ColorLib.TenebrisGradient * 0.6f, 1f, 0.2f, 120);
                    ParticleEngine.BehindProjectiles.Add(Cloud);
                }
            }

            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, ColorLib.TenebrisGradient, 0.5f, 40, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire2);

			float maxDetectRadius = 1400f;

			HomeTimer++;
			
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

            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                Fire fire = new Fire();
                fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, ColorLib.TenebrisGradient * 0.5f, 0.8f, 40, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                ParticleEngine.BehindProjectiles.Add(fire);

                if (Main.rand.NextBool(2))
                {
                    TenebrousCloudParticle Cloud = new();
                    Cloud.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.06f, ColorLib.TenebrisGradient * 0.6f, 1f, 0.2f, 120);
                    ParticleEngine.BehindProjectiles.Add(Cloud);
                }
            }

            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, ColorLib.TenebrisGradient, 0.5f, 40, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire2);
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