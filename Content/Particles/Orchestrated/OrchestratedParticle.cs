using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Orchestrated
{
    public abstract class OrchestratedParticle : BasePRT
    {
        public virtual string ParticleName {get; protected set;}
        public virtual Color[] Colors => new Color[99];
        public virtual int MaxLifetime {get; protected set;}
        public override void SetProperty()
        {
            Lifetime = MaxLifetime; 
            ShouldKillWhenOffScreen = false;
        }
        public override void AI()
        {
            
        }
        public override bool PreDraw(SpriteBatch spriteBatch) => false;
    }
}