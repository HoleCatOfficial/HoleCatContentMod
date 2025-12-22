using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class Boom : BasePRT
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
            float growSpeed = 0.02f; // how fast it grows each tick

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

    internal class Boom1 : Boom
    {
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = 9999;
            Scale = 0.01f;
        }
    }

    internal class Boom2 : Boom
    {
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = 9999;
            Scale = 0.01f;
        }
    }

    internal class Boom3 : Boom
    {
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = 9999;
            Scale = 0.01f;
        }
    }

    internal class Boom4 : Boom
    {
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = 9999;
            Scale = 0.01f;
        }
    }
    internal class Boom5 : Boom
    {
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = 9999;
            Scale = 0.01f;
        }
    }

}