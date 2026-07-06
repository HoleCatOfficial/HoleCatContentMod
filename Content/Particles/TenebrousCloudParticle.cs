using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
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

namespace DestroyerTest.Content.Particles
{
    public class TenebrousCloudParticle : BaseParticle<PointGlow>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 120;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        float scale;
        public float _scale;

        float Rotation;
        float Opacity;

        public float _Opacity;

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Opacity, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.Opacity = this._Opacity = Opacity;
            this.scale = this._scale = Scale;
        }

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Opacity, float Scale, int Lifetime)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = this._scale = Scale;
            this.Opacity = this._Opacity = Opacity;
            this.MaxLifetime = Lifetime;
        }


        public override void Update(ref ParticleRendererSettings settings)
        {
            float Progress = (float)Lifetime / (float)MaxLifetime;
            Lifetime++;
            position += velocity;

            Rotation += (velocity.X * 0.1f) * Math.Sign(velocity.X);

            Opacity = MathHelper.Lerp(_Opacity, 0, Progress);

            scale = MathHelper.Lerp(_scale, 0.1f, Progress);

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

    
      
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/TenebrousCloudParticle").Value;
            Vector2 origin = texture.Size() / 2f;

            var Cap = spriteBatch.Capture();

            spriteBatch.UseBlendState(BlendState.AlphaBlend);

            spriteBatch.End();
            spriteBatch.Begin(Cap);
            //Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 } * Opacity, Rotation, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefault();
        }
    }
}
