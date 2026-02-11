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

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
	public class StellarFlameFriendly : ModProjectile
	{
		public override string Texture => DTUtils.NoTexture;

		public override void SetDefaults()
		{
			Projectile.width = 80; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 240; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.alpha = 160;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero,  ColorLib.StellarFireGradientLooping() * 0.75f, 1.5f);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Vector2.Zero,  ColorLib.StellarFireGradientLooping() * 0.5f, 2.5f, 40, ai2: 2);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Vector2.Zero,  ColorLib.StellarFireGradientLooping() * 0.1f, 1f, 40, ai2: 2);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 120);
		}
	}

	
}