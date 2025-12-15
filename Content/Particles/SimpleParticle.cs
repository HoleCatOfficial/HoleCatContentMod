using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class SimpleParticle : BasePRT
    {
        public int MaxLifetime => 40;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            Scale += Main.rand.NextFloat(0.15f, 0.5f);
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            if (LifetimeCompletion > 0.3f)
            {
                Color *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return true;
        }
    }
}