using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class SoundwaveParticle : BasePRT
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
            float endScale = ai[0];
            float growSpeed = 0.07f;

            if (Scale < endScale)
            {
                Scale += growSpeed;
            }

            float fadeStart = endScale * 0.8f;
            if (Scale >= fadeStart)
            {
                Color *= 0.9f;
            }

            if (Scale >= endScale)
                Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}