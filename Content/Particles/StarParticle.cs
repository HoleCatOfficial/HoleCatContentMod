using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
 
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class StarParticle : BaseParticle<StarParticle>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 100;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;
        public float Rotation = 0f;
        public float RotationAmount = 0f;
        bool Rotates = false;

        public int Style = 0;
        public BlendState blendState = BlendState.Additive;

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, int Lifetime = 100, int Style = 1)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
            this.Style = Style;
            this.Lifetime = 0;
            this.MaxLifetime = Lifetime;
        }


        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, float RotationSpeed, int Lifetime = 100, int Style = 1)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
            this.RotationAmount = RotationSpeed;
            this.Rotates = true;
            this.Lifetime = 0;
            this.MaxLifetime = Lifetime;
            this.Style = Style;
        }

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, BlendState BlendState, int Lifetime = 100, int Style = 1)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
            this.Lifetime = 0;
            this.MaxLifetime = Lifetime;
            this.blendState = BlendState;
            this.Style = Style;
        }


        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, float RotationSpeed, BlendState BlendState, int Lifetime = 100, int Style = 1)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
            this.RotationAmount = RotationSpeed;
            this.Rotates = true;
            this.Lifetime = 0;
            this.MaxLifetime = Lifetime;
            this.blendState = BlendState;
            this.Style = Style; 
        }


        float LifetimeCompletion => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            position += velocity;

            if (Rotates)
            {
                Rotation += RotationAmount;
            }

            if (LifetimeCompletion > 0.5f)
            {
                color *= 0.9f;
                scale *= 0.97f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            string Add = blendState == BlendState.Additive ? "_Add" : "";
            Texture2D texture = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/StarParticle{Style}{Add}").Value;
            Vector2 origin = texture.Size() / 2f;

            spriteBatch.UseBlendState(blendState);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color, Rotation, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefault();
        }
    }
}