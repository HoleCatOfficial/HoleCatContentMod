using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
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
        public int MaxLifetime = 120;
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
        float Progress => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            position += velocity;

            if (Progress > 0.5f)
            {
                Opacity -= 0.01f;
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
            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(DTAssetLib.PointGlow.Value, position - Main.screenPosition, null, ColorLib.Rift with { A = 0 } * Opacity, 0f, DTAssetLib.PointGlow.Value.Size() / 2f, new Vector2(Scale1, Scale1), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Sparkle(5).Value, position - Main.screenPosition, null, Color.White with { A = 0 } * Opacity, 0f, DTAssetLib.Sparkle(5).Value.Size() / 2f, new Vector2(Scale2, Scale2), SpriteEffects.None);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}