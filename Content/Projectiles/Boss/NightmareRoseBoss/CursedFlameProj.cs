using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
	public class CursedFlameProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 80; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.3f; // How much light emit around the projectile
			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}



		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, ColorLib.WretchedColorMap, 1f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);
        }

		

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<Defilement>(), 300);
		}
	}

}