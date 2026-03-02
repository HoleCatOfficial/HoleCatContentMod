using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
	public class PoisonCloud : ModProjectile
	{
        private void AnimateProjectile() 
        {
            if (++Projectile.frameCounter >= 3) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        public override void AI() 
        {
            AnimateProjectile();
            
            Projectile.velocity *= 0.95f;

            if (Projectile.timeLeft <= 30)
            {
                Projectile.Opacity *= 0.8f;
                Projectile.scale *= 1.01f;
            }
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }
	}
}