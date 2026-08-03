using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
 
using DestroyerTest.Content.Particles;
using DestroyerTest.Common;
using Microsoft.Build.Execution;
using DestroyerTest.Content.Buffs;
using Terraria.ID;
using DestroyerTest.Content.Dusts;
using FullSerializer;
using OpusLib;
using OpusLib.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using BreadLibrary.Core.Graphics.Particles;

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

        }

        public override void AI()
        {
            AnimateProjectile();

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<SoulDust>(), 0, 0, 0, ColorLib.Soul * 0.5f, 5f);
            Projectile.velocity = new Vector2(0, -0.4f);
        }

        public override void OnKill(int timeLeft)
        {
            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, Color.White, 0.2f, 0.01f, 2f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);
            
        }

        
    }
}
