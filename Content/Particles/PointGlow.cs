using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class PointGlow : BaseParticle<PointGlow>, IDrawPixelated
    {
        public int Lifetime = 0;
        public int MaxLifetime = 120;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
        }

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, int Lifetime)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
            this.MaxLifetime = Lifetime;
        }


        public override void Update(ref ParticleRendererSettings settings)
        {
            float Progress = (float)Lifetime / MaxLifetime;
            Lifetime++;
            position += velocity;

            if (Progress > 0.5f)
            {
                color *= 0.95f;
                scale *= 0.95f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        PixelLayer IDrawPixelated.PixelLayer => DefaultPixelLayer;
        bool IDrawPixelated.ShouldDrawPixelated => true;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/PointGlow").Value;
            Vector2 origin = texture.Size() / 2f;

            var Cap = spriteBatch.Capture();

            spriteBatch.UseBlendState(BlendState.Additive);
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);
            //Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefault();
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
           
        }
    }

    public class PointGlowPreMultiplied : PointGlow, IDrawPixelated
    {
        PixelLayer IDrawPixelated.PixelLayer => DefaultPixelLayer;
        bool IDrawPixelated.ShouldDrawPixelated => true;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/PointGlow").Value;
            Vector2 origin = texture.Size() / 2f;

            var Cap = spriteBatch.Capture();

            Cap.BlendState = BlendState.Additive;

            spriteBatch.UseBlendState(BlendState.Additive);
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);
            //Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefault();
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            
        }
    }
}
