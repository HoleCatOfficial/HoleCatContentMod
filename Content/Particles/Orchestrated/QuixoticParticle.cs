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

namespace DestroyerTest.Content.Particles.Orchestrated
{
    public class  QuixoticParticle : BaseParticle<QuixoticParticle>
    {
        public Vector2 position;
        public Color color = new Color(255, 219, 6);

        public bool Spawned = false;

        public void Initiate(Vector2 Position)
        {
            this.position = Position;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (!Spawned)
            {
                QuixoticSpark FX1 = new QuixoticSpark();
                FX1.Prepare(position, Vector2.Zero, color, 1f);
                ParticleEngine.BehindProjectiles.Add(FX1);

                QuixoticSpark2 FX2 = new QuixoticSpark2();
                FX2.Prepare(position, Vector2.Zero, color, 2f);
                ParticleEngine.BehindProjectiles.Add(FX2);

                Spawned = true;
            }
            else
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
    }

    public class QuixoticSpark : BaseParticle<QuixoticSpark>
    {
        int Lifetime = 0;
        int MaxLifetime = 20;
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

        float LifetimeCompletion => (float)Lifetime / (float)MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            _scale = MathHelper.Lerp(0f, scale, Utilities.Convert01To010(LifetimeCompletion));
            rotation += 0.1f;
            position += velocity;
            

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = DTAssetLib.MiscSparkle144.Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, rotation, origin, _scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }

    public class QuixoticSpark2 : BaseParticle<QuixoticSpark2>
    {
        int Lifetime = 0;
        int MaxLifetime = 20;
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

        float LifetimeCompletion => (float)Lifetime / (float)MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            _scale = MathHelper.Lerp(0f, scale, Utilities.Convert01To010(LifetimeCompletion));
            position += velocity;
           

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = DTAssetLib.MiscSparkle144.Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, rotation, origin, _scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}