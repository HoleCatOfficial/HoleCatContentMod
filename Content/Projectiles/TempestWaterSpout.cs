using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class TempestWaterSpout : ModProjectile
	{
        public override void SetStaticDefaults()
        {
		}

        public override void SetDefaults()
        {
            Projectile.width = 40; // The width of projectile hitbox
            Projectile.height = 90; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
		}


        public override void AI()
        {
            Vector2 Out = new Vector2(Main.rand.Next(-4, 4), 35);
            Vector2 BottomCenter = new Vector2(Projectile.Center.X, Projectile.Center.Y + (Projectile.height / 2));
            Vector2 Dir = Out - BottomCenter;

            for (int t = 0; t < 36; t++)
            {
                Dust.NewDustPerfect(BottomCenter, DustID.Water, Dir, 100, default, 4f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Wet, 300);
        }
	}
}