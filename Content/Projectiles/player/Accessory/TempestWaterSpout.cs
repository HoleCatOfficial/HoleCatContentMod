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

namespace DestroyerTest.Content.Projectiles.player.Accessory
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.alpha = 255;
		}


        public override void AI()
        {
            Vector2 Out = Projectile.Bottom + new Vector2(Main.rand.NextFloat(-2, 2), -6);
            Vector2 BottomCenter = Projectile.Bottom;
            Vector2 Dir = Out - BottomCenter;

            for (int t = 0; t < 36; t++)
            {
                Dust.NewDustPerfect(BottomCenter, DustID.Water_Snow, Dir, 100, default, 3f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Wet, 300);
        }
	}
}