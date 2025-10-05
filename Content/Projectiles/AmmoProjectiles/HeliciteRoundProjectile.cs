using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
	// This example is similar to the Wooden Arrow projectile
	public class HeliciteRoundProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 68; // The width of projectile hitbox
			Projectile.height = 2; // The height of projectile hitbox
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 300);
        }

		public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++) // Creates a splash of dust around the position the projectile dies.
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftDust>());
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }
	}
}