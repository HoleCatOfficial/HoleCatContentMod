using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class FungusSporeExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void OnSpawn(IEntitySource source)
        {
            int[] puffball = new int[] { 375, 376, 377 };
            SoundEngine.PlaySound(SoundID.Item42);
            for (int g = 0; g < 8; g++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Main.rand.NextVector2Circular(10, 10), new Color(63, 66, 207) * 0.5f, 1f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Main.rand.NextVector2Circular(10, 10), new Color(63, 66, 207), 0.5f);
                Gore.NewGorePerfect(source, Projectile.Center, Main.rand.NextVector2Circular(6, 6), puffball[Main.rand.Next(puffball.Length)]);
            }
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.Kill();
                }
            }
        }
    }
}