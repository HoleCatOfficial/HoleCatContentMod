using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class StellarPointGlow : PointGlowPreMultiplied
    {
        public void Prepare(Vector2 Position, Vector2 Velocity)
        {
            position = Position;
            velocity = Velocity;
        }
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;

            float LifetimeCompletion = (float)Lifetime / MaxLifetime;

            color = DTColorUtils.MultiLerp(LifetimeCompletion, ColorLib.StellarFireColormap);
            position += velocity;

            if (LifetimeCompletion > 0.5f)
            {
                color *= 0.95f;
                scale *= 0.95f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/PointGlowPreMultiplied").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, null, OpusColorUtils.Pastel(color, 0.5f) with { A = 0 }, 0f, origin, scale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, null, OpusColorUtils.Pastel(color, 0.8f) with { A = 0 }, 0f, origin, scale * 0.3f, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
 
}