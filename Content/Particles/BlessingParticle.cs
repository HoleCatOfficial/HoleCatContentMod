using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace DestroyerTest.Content.Particles
{
    public class BlessingParticle : BasePRT
    {
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = 9999;
            Scale = 0.01f;
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            float endScale = ai[0]; // allow dynamic sizing
            float growSpeed = 0.04f; // how fast it grows each tick

            if (Scale < endScale)
            {
                Scale += growSpeed;
            }

            float fadeStart = endScale * 0.8f;
            if (Scale >= fadeStart)
            {
                Color *= 0.9f;
            }

            // Kill once scale is basically done growing
            if (Scale >= endScale)
                Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}
