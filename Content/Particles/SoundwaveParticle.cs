using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class SoundwaveParticle : BaseParticle<SoundwaveParticle>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 20;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;
        float rotation;
        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;

            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        float Progress => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            position += velocity;

            float scaleRate = MathHelper.Lerp(0.1f, 0.02f, Progress);
            scale += scaleRate;

            if (Progress > 0.5f)
            {
                color *= 0.95f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer PixelLayer => PixelLayer.AboveNPCs;

        public override bool DrawsPixelated => true;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SoundwaveParticle").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, 0f, origin, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}