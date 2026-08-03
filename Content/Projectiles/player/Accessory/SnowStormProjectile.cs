using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
	public class SnowStormProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 480;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			Projectile.penetrate = 2;
		}



        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.Snow, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default, 2f);
			Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.Frost, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default, 2f);
		}



		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<HaepiensBlizzard>(), 120);
		}
	}

}