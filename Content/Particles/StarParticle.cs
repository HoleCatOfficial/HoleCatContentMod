using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace DestroyerTest.Content.Particles
{
    internal class StarParticle : BasePRT
    {
        public int MaxLifetime => 60;
        public override void SetProperty()
        {
            
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
        }

        public override void AI()
        {
            Scale *= 0.995f;
            Velocity *= 0.999f;
            if (LifetimeCompletion > 0.5f)
            {
                Color *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}