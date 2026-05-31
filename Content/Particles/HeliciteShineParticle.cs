using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Magic;
  
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class HeliciteShineParticle : BaseParticle<HeliciteShineParticle>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 300;
        public Vector2 position;
        public Vector2 velocity;
        float Opacity = Main.rand.NextFloat(0.5f, 1.1f);

        public void Initialize(Vector2 Position, Vector2 Velocity)
        {
            this.position = Position;
            this.velocity = Velocity;
        }

        public float Scale1 = 0f;
        public float Scale2 = 0f;
        
        public override void Update(ref ParticleRendererSettings settings)
        {
            float Progress = (float)Lifetime / (float)MaxLifetime;

            Scale1 = Opus.Sine(0.5f, 0.8f, 0.01f);
            Scale2 = Opus.Sine(0.1f, 0.5f, 0.2f);

            Lighting.AddLight(position, ColorLib.DarkRift3.ToVector3() * Scale2);

            Lifetime++;
            position += velocity;

            if (Lifetime > 200)
            {
                Opacity -= 0.01f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AbovePlayer;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, position - Main.screenPosition, null, ColorLib.Rift with { A = 0 } * Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2f, new Vector2(Scale1, Scale1), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Sparkle(5, true).Value, position - Main.screenPosition, null, Color.White with { A = 0 } * Opacity, 0f, DTAssetLib.Sparkle(5, true).Value.Size() / 2f, new Vector2(Scale2, Scale2), SpriteEffects.None);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}