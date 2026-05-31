using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace DestroyerTest.Content.Particles
{
    public class HallowedPallStar : BaseParticle<HallowedPallStar>
    {
        int Lifetime = 0;
        int MaxLifetime = 120;
        Vector2 position;
        Vector2 velocity;
        Color color;
        float scale;
        float rotation;

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
        }

        float LifetimeCompletion => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            position += velocity;
            velocity *= 0.99f;
            if (velocity.X > 0)
            {
                rotation += 0.1f;
            }
            if (velocity.X < 0)
            {
                rotation -= 0.1f;
            }
            if (LifetimeCompletion > 0.6f)
            {
                color *= 0.85f;
                scale *= 0.85f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/HallowedPallStar").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color, rotation, origin, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}