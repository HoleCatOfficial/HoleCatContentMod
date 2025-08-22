using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	// This example is similar to the Wooden Arrow projectile
	public class BloodRain : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30; // The width of projectile hitbox
			Projectile.height = 2; // The height of projectile hitbox
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.timeLeft = 1200;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public override void AI() {
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] >= 5f) {
				Projectile.ai[0] = 5f;
				Projectile.velocity.Y += 0.1f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.velocity.Y > 16f) {
				Projectile.velocity.Y = 16f;
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return true;
        }
	}
}