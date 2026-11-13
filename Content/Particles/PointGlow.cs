using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class PointGlow : BaseParticle<PointGlow>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 120;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;

        public float InitScale = 0f;

        public bool shrink = false;

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = this.InitScale = Scale;
            
        }

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, int Lifetime)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = this.InitScale = Scale;
            this.MaxLifetime = Lifetime;
        }

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale, int Lifetime, bool Shrink)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = this.InitScale = Scale;
            this.MaxLifetime = Lifetime;
            this.shrink = Shrink;
        }


        public override void Update(ref ParticleRendererSettings settings)
        {
            float Progress = (float)Lifetime / MaxLifetime;
            Lifetime++;
            position += velocity;

            if (shrink)
            {
                scale = MathHelper.Lerp(InitScale, 0f, Progress);

                if (Progress > 0.5f)
                {
                    color *= 0.95f;
                }
            }
            else
            {

                if (Progress > 0.5f)
                {
                    color *= 0.95f;
                    scale *= 0.95f;
                }
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/PointGlow").Value;
            Vector2 origin = texture.Size() / 2f;

            var Cap = spriteBatch.Capture();

            spriteBatch.UseBlendState(BlendState.Additive);

            //Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(Cap);
        }
    }

    public class PointGlowPreMultiplied : PointGlow
    {

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/PointGlowPreMultiplied").Value;
            Vector2 origin = texture.Size() / 2f;

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, 0f, origin, scale, SpriteEffects.None, 0f);

        }
    }
}
