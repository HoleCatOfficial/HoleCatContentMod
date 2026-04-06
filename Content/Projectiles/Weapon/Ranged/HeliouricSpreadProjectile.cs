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

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
	public class HeliouricSpreadProjectile : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;

			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
		}



		public override void AI()
		{
			for (int i = 0; i < 2; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, ColorLib.Rift, 1f);
			}
			if (!DTOptimizationsConfig.instance.DisableExcessParticles)
			{
                PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Projectile.Center, Vector2.Zero, ColorLib.Rift * 0.25f, 0.5f);
                PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Projectile.Center, Vector2.Zero, ColorLib.Rift * 0.75f, 0.35f);
            }
			else
			{
				if (Main.rand.NextBool(20))
				{
                    PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Projectile.Center, Vector2.Zero, ColorLib.Rift * 0.25f, 0.5f);
                    PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Projectile.Center, Vector2.Zero, ColorLib.Rift * 0.75f, 0.35f);
                }
			}
		}

		

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
		}
	}

}