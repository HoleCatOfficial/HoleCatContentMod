using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	// This example is similar to the Wooden Arrow projectile
	public class TenebrisFireBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox
			Projectile.tileCollide = true; // Allow the projectile to collide with tiles
			Projectile.friendly = false;
			Projectile.hostile = true; // Make it hostile, so it can damage players
			Projectile.DamageType = DamageClass.Generic;
			Projectile.timeLeft = 600;
		}

		public override void AI() {
			Projectile.rotation += 0.4f * Projectile.direction;

			int[] types = new int[]
			{
				PRTLoader.GetParticleID<ColoredFire1>(),
				PRTLoader.GetParticleID<ColoredFire2>(),
				PRTLoader.GetParticleID<ColoredFire3>(),
				PRTLoader.GetParticleID<ColoredFire4>(),
				PRTLoader.GetParticleID<ColoredFire5>(),
				PRTLoader.GetParticleID<ColoredFire6>(),
				PRTLoader.GetParticleID<ColoredFire7>()
			};
			PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Projectile.Center, new Vector2(0f, -0.1f), ColorLib.TenebrisGradient, 1.0f);
			if (TileCollideCount <= 0)
			{
				Projectile.Kill();
			}
		}

		public int TileCollideCount = 6;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
			TileCollideCount--;
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

			// If the projectile hits the left or right side of the tile, reverse the X velocity
			if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}

			// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
			if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}

		public override void OnKill(int timeLeft) {
			for (int i = 0; i < 5; i++) // Creates a splash of dust around the position the projectile dies.
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WaterCandle);
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			} 
		}
	}
}