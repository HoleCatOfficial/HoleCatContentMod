using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Orchestrated
{
    public class GargantuaParticle : BasePRT
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
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0, -1), Color.Red, 1f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0, 1), Color.Red, 1f);

                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0.5f, 0), Color.Red, 0.5f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(-0.5f, 0), Color.Red, 0.5f);
                Spawned = true;
            }            
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return false;
        }
    }
}