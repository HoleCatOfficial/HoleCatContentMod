using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Orchestrated
{
    public class ColossusParticle : BasePRT
    {
        public override void SetProperty()
        {
            Lifetime = 60;
            ShouldKillWhenOffScreen = false;
        }

        public bool Spawned = false;
        public override void AI()
        {
            if (!Spawned)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Position, Vector2.Zero, Color.White, 1f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0, -1), ColorLib.TenebrisMagenta, 1f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0, 1), ColorLib.TenebrisMagenta, 1f);

                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0.5f, 0), ColorLib.TenebrisMagenta, 0.5f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(-0.5f, 0), ColorLib.TenebrisMagenta, 0.5f);
                Opus.RadialSpreadParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 4, Position, 1, DTColorUtils.Pastel(ColorLib.TenebrisMagenta, 0.9f), 0.2f, 1f, offset: MathHelper.PiOver4);
                Spawned = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return false;
        }
    }
}