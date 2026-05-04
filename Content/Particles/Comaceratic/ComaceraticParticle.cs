using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using InnoVault.PRT;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Comaceratic
{
    public class ComaceraticParticle : BaseParticle<ComaceraticParticle>
    {
        int Lifetime = 0;
        int MaxLifetime = 75;
        Vector2 position;
        Vector2 velocity;
        Color color;
        float scale;
        float rotation;

        int variant => Main.rand.Next(3);
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
            velocity *= 0.96f;
            if (velocity.X > 0)
            {
                rotation += 0.06f;
            }
            if (velocity.X < 0)
            {
                rotation -= 0.06f;
            }
            if (LifetimeCompletion > 0.6f)
            {
                color *= 0.9f;
                scale *= 0.9f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
        public override PixelLayer PixelLayer => PixelLayer.AboveProjectiles;
        public override bool DrawsPixelated => true;
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/Comaceratic/ComaceraticParticle{variant}").Value;
            Vector2 origin = texture.Size() / 2f;
            spriteBatch.Draw(
                texture,
                position - Main.screenPosition,
                null,
                color,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
