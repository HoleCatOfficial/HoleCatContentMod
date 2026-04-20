using System;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class BloomRingSharp : BasePRT
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
            float growSpeed = MathHelper.Lerp(0.08f, 0.01f, Scale / endScale); // how fast it grows each tick

            if (Scale < endScale)
            {
                Scale += growSpeed;
            }

            float fadeStart = endScale * 0.5f;
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


    public class BloomRingZap : BasePRT
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
    
    public class BloomRing : BasePRT
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

    
}