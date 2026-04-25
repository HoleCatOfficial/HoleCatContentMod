using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
	public class StellarFlameHostile : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;

		public override void SetDefaults()
		{
			Projectile.width = 80; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.alpha = 160;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public override void AI()
		{
            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, ColorLib.StellarFireColormap, 3f, 100, FireDrawMode.Additive);
			ParticleEngine.BehindProjectiles.Add(fire);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 120);
		}
	}

	
}