using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class ImpactCracks : BaseParticle<ImpactCracks>
    {
        int Lifetime = 0;
        int MaxLifetime = 30;
        Vector2 position;
        Color color;
        float scale;
        float rotation;

        public void Prepare(Vector2 Position, Color Color, float Scale)
        {
            this.position = Position;
            this.color = Color;
            this.scale = Scale;

            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        float LifetimeCompletion => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
     
            if (LifetimeCompletion > 0.3f)
            {
                color *= 0.85f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/PreMultiplied/ImpactCracks").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0}, rotation, origin, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}