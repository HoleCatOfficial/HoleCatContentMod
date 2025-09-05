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
	public class BloodCloud : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
		}

        public override void SetDefaults()
        {
            Projectile.width = 54; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
		}

        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 4) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public override void AI()
        {
            AnimateProjectile();
            if (Main.rand.NextBool(4))
            {
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(0, 32), ModContent.ProjectileType<BloodRain>(), 15, 1);
            }
        }
	}
}