using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class CursedFlamesFriendly : ModProjectile
	{





		public override void SetDefaults()
		{
			Projectile.width = 80; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			Projectile.penetrate = 2;
		}



		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			int[] types = new int[]
			{
				PRTLoader.GetParticleID<CursedFlame2Particle1>(),
				PRTLoader.GetParticleID<CursedFlame2Particle2>(),
				PRTLoader.GetParticleID<CursedFlame2Particle3>(),
				PRTLoader.GetParticleID<CursedFlame2Particle4>(),
				PRTLoader.GetParticleID<CursedFlame2Particle5>(),
				PRTLoader.GetParticleID<CursedFlame2Particle6>(),
				PRTLoader.GetParticleID<CursedFlame2Particle7>()
			};

			PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Projectile.Center, Vector2.Zero, ColorLib.CursedFlames, 0.5f);
			
			
			
		}

		

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.CursedInferno, 120);
		}
	}

}