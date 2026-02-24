using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace DestroyerTest.Content.Particles
{
    public class HallowedPallStar : BasePRT
    {
        public int MaxLifetime => 120;
        public override void SetProperty()
        {
            
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
        }

        public override void AI()
        {
            Velocity *= 0.99f;
            if (Velocity.X > 0)
            {
                Rotation += 0.1f;
            }
            if (Velocity.X < 0)
            {
                Rotation -= 0.1f;
            }
            if (LifetimeCompletion > 0.6f)
            {
                Color *= 0.9f;
                Scale *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}