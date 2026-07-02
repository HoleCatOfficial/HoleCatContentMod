using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
	public class HeliciteRoundProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        float Scl = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5, true), Projectile, Color.White with { A = 0 }, false, 0f, Scl, Scl);
            return false;
        }
        
		public override void AI() 
        {
            Scl = Opus.Sine(0.8f, 0.4f, 0.6f);
            var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, ColorLib.Rift, 1f);
            d.noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 300);
        }

		public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RiftDust>());
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }
	}
}