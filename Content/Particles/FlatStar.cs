using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class FlatStar : BaseParticle<FlatStar>
    {
        int Lifetime = 0;
        int MaxLifetime = 30;
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

            _scale = MathHelper.Lerp(0f, scale, Utilities.Convert01To010(LifetimeCompletion));
            rotation += 0.1f;
            position += velocity;
            Lifetime++;

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer PixelLayer => PixelLayer.AboveProjectiles;

        public override bool DrawsPixelated => true;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/FlatStar").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, rotation, origin, _scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }


    public class FlatStarStellar : BaseParticle<FlatStarStellar>
    {
        int Lifetime = 0;
        int MaxLifetime = 30;
        Vector2 position;
        Vector2 velocity;
        Color color;
        float _scale = 0f;
        float scale;
        float rotation;

        public void Prepare(Vector2 Position, Vector2 Velocity, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.scale = Scale;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            float LifetimeCompletion = (float)Lifetime / MaxLifetime;

            color = OpusColorUtils.MultiLerp(LifetimeCompletion, ColorLib.StellarFireColormap);
            _scale = MathHelper.Lerp(0f, scale, Utilities.Convert01To010(LifetimeCompletion));
            rotation += 0.1f;
            position += velocity;
            Lifetime++;

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer PixelLayer => PixelLayer.AboveProjectiles;

        public override bool DrawsPixelated => true;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/FlatStar").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, rotation, origin, _scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}
