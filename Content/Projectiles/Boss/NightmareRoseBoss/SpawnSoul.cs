using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Common;
using Microsoft.Build.Execution;
using DestroyerTest.Content.Buffs;
using Terraria.ID;
using DestroyerTest.Content.Dusts;
using FullSerializer;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class SpawnSoul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 8;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int c = 0; c < 4; c++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom3>(), Projectile.Center, Vector2.Zero, ColorLib.Soul, 0.01f, 1);
            }
        }

        public override void AI()
        {
            AnimateProjectile();
            for (int g = 0; g < 4; g++)
            {
                Vector2 Outer = Projectile.Center + Main.rand.NextVector2CircularEdge(800, 800);
                Vector2 Dir = Projectile.Center - Outer;
                PRTLoader.NewParticle(PRTLoader.GetParticleID<TormentedSoulParticle>(), Outer, Dir * 0.01f, Color.White, Main.rand.NextFloat(0.5f, 2.5f));
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Outer, Dir * 0.01f, ColorLib.Soul, Main.rand.NextFloat(0.5f, 4.5f));
            }

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<SoulDust>(), 0, 0, 0, ColorLib.Soul * 0.5f, 5f);
            Projectile.velocity = new Vector2(0, -0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, Color.White, 0.01f, ai0: 2.6f);
        }

        
    }
}
