using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace DestroyerTest.Content.Particles
{
    internal class CrimsonBloodRuneParticle : BasePRT
    {

        public int MaxLifetime => 120;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Lifetime = MaxLifetime;
            Scale = 0.01f;
            Color = Color.White;
        }


        public override void AI()
        {
            Velocity *= 0.99f;
            Rotation += Velocity.ToRotation();

            // Smooth lifetime-based scale growth:
            float t = LifetimeCompletion; // 0 → 1 across particle lifetime
            float startScale = 0.01f;
            float endScale = Scale * 1.15f; // 3x initial scale
            float easedT = MathF.Pow(t, 0.5f); // fast linearization for decaying visuals
            float currentScale = MathHelper.Lerp(startScale, endScale, easedT);
            Scale = currentScale;

            // Optional fading near the end of life
            if (t > 0.8f)
                Color *= 0.9f;

            // Kill particle when its lifetime runs out
            if (Lifetime <= 0)
                Kill();
        }


        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}