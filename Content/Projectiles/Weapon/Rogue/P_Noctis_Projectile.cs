using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RogueItems;
using Microsoft.Xna.Framework;
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
			Projectile.width = 132;
			Projectile.height = 132;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.penetrate = 4;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 2;
		}

		public bool Dying = false;

        public override void AI() 
		{
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

			if (Dying)
			{
				Projectile.velocity *= 0.95f;
                return;
            }

			Rectangle DustBox = Utils.CenteredRectangle(Projectile.Center, new Vector2(16, 16));


			Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(DustBox), DustID.WaterCandle, Projectile.velocity * 0.2f, 0, default, 2f);
            Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(DustBox), DustID.WaterCandle, Projectile.velocity * 0.2f, 100, default, 2.6f);


        }

	

		public override void OnKill(int timeLeft) 
		{
			Vector2 usePos = Projectile.position; 

			Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2();
			usePos += rotationVector * 16f;

			for (int i = 0; i < 20; i++) {
				Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Bone);
				dust.position = (dust.position + Projectile.Center) / 2f;
				dust.velocity += rotationVector * 2f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
				usePos -= rotationVector * 0f;
			}
			
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			if (!Dying)
			{
				SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing);
				WitheringSpark Spark = new WitheringSpark();
				Spark.PrepareSpark(target.Center, Projectile.velocity * 0.01f, Projectile.rotation, Color.Blue, 2f, false, 80, SparkDrawMode.AlphaBlend, 3f);
				ParticleEngine.Particles.Add(Spark);
				Dying = true;
                Projectile.netUpdate = true;
            }
			
		

		}
	}
}