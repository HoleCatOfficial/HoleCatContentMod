using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike;
using DestroyerTest.Content.RogueItems;
using Microsoft.Xna.Framework;
using OpusLib;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
	public class P_Noctis_Projectile : ModProjectile
	{
		

		public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.penetrate = -1;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 2;
		}

		Color DColor = Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, DColor));
			return false;
        }

		public bool Dying = false;

        public override void AI() 
		{
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            Rectangle DustBox = Utils.CenteredRectangle(Projectile.Center, new Vector2(16, 16));

            OutlineCircleParticle Particle = new OutlineCircleParticle();
            Particle.Create(Main.rand.NextVector2FromRectangle(DustBox), Projectile.velocity * 0.2f, Color.Blue, 1.5f);
            ParticleEngine.BehindProjectiles.Add(Particle);

            Dust F = Dust.NewDustPerfect(Projectile.Center + new Vector2(-6, -6).RotatedBy(Projectile.rotation), DustID.Torch, Projectile.velocity * 0.2f, 0, default, 1.2f);
            Dust I = Dust.NewDustPerfect(Projectile.Center + new Vector2(6, 6).RotatedBy(Projectile.rotation), DustID.IceTorch, Projectile.velocity * 0.2f, 100, default, 1.2f);
			F.noGravity = I.noGravity = true;

            if (Dying)
			{
				Projectile.velocity *= 0.95f;
				Projectile.ai[0]++;

				DColor = Color.Lerp(Color.White, Color.Black, Projectile.ai[0] / 120f);

                if (Projectile.ai[0] >= 120)
				{
                    Projectile.Kill();
                }
                return;
            }
			else
			{
				DColor = Color.White;
			}

			

           


        }



		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.EnergyWoosh with { Volume = 0.85f }, Projectile.Center);

			for (int i = 0; i < 16; i++)
			{
				OutlineCircleParticle Particle = new OutlineCircleParticle();
				Particle.Create(Projectile.Center, Main.rand.NextVector2Circular(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)), Color.Blue, 2f);
				ParticleEngine.BehindProjectiles.Add(Particle);
			}
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			if (!Dying)
			{
				SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing);
				WitheringSpark Spark = new WitheringSpark();
				Spark.PrepareSpark(Projectile.Center, Projectile.velocity * 0.01f, Projectile.rotation, Color.Blue, 2f, false, 80, SparkDrawMode.Additive, 3f);
				ParticleEngine.Particles.Add(Spark);
				Dying = true;
                Projectile.netUpdate = true;
            }

			target.AddBuff(ModContent.BuffType<HaepiensInferno>(), 300);
            target.AddBuff(ModContent.BuffType<HaepiensBlizzard>(), 300);

			if (Projectile.StealthStrike(Main.player[Projectile.owner]))
			{
				var P = Opus.RandomRingVectors(4, target.Center, 200);

				for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), P[i], Vector2.Zero, ModContent.ProjectileType<PNoctisSparkFire>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, target.whoAmI);
                }
                for (int i = 2; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), P[i], Vector2.Zero, ModContent.ProjectileType<PNoctisSparkIce>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, target.whoAmI);
                }
            }


        }
	}
}