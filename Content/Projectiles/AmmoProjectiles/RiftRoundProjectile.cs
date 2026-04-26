using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
	public class RiftRoundProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
			return false;
        }

		public override void AI() 
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
        }

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftDust>());
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			}
		}
	}
}