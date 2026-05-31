using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
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
    public class SmallShine : BaseParticle<SmallShine>
    {
        int Lifetime = 0;
        int MaxLifetime = 40;
        Vector2 position;
        Vector2 velocity;
        Color color;
        float _scale = 0f;
        float scale;
        float rotation;

        public void Prepare(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            float LifetimeCompletion = (float)Lifetime / MaxLifetime;
            _scale = MathHelper.Lerp(0, scale, Utilities.Convert01To010(LifetimeCompletion));
            rotation += 0.1f;
            position += velocity;
            Lifetime++;

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SmallShine").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, rotation, origin, _scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}
