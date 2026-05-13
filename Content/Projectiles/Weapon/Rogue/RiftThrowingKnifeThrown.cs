using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
	public class RiftThrowingKnifeThrown : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
			Projectile.timeLeft = 1200;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			Projectile.extraUpdates = 40;
			Projectile.penetrate = 3;
		}

		public override void AI() 
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.3f, 0, ColorLib.DarkRift2, 0.3f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			for (int i = 0; i < 5; i++)
			{
				Rectangle R = Utils.CenteredRectangle(Projectile.Center, new Vector2(10, 10));
				Vector2 Vel = Projectile.velocity.RotatedByRandom(0.07f);
				Dust.NewDustDirect(R.TopLeft(), 10, 10, DustID.FireworksRGB, Vel.X, Vel.Y * 0.5f, 0, ColorLib.Rift, 0.5f);
			}

			if (hit.Crit)
			{
				SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);

                Rectangle R = Utils.CenteredRectangle(Projectile.Center, new Vector2(10, 10));
                Vector2 Vel = Projectile.velocity.RotatedByRandom(0.07f);
                Dust.NewDustDirect(R.TopLeft(), 10, 10, DustID.FireworksRGB, Vel.X, Vel.Y * 0.5f, 0, ColorLib.LightRift2, 0.75f);

				target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
            }
		}


		public override void OnKill(int timeLeft) 
		{
            Rectangle R = Utils.CenteredRectangle(Projectile.Center, new Vector2(10, 10));
            Vector2 Vel = Projectile.velocity.RotatedByRandom(0.07f);
            Dust.NewDustDirect(R.TopLeft(), 10, 10, DustID.FireworksRGB, Vel.X, Vel.Y * 0.5f, 0, ColorLib.Rift, 0.5f);
        }
	}
}