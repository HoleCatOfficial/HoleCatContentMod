using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class WretchedPointGlow : PointGlowPreMultiplied
    {
        List<Vector2> OldPositions = new();
        public void Prepare(Vector2 Position, Vector2 Velocity, float Scale)
        {
            position = Position;
            velocity = Velocity;
            scale = Scale;
        }
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;

            float LifetimeCompletion = (float)Lifetime / MaxLifetime;

            color = DTColorUtils.MultiLerp(LifetimeCompletion, ColorLib.WretchedColorMap);
            position += velocity;

            OldPositions.Add(position);

            if (OldPositions.Count > 12)
            {
                OldPositions.RemoveAt(0);
            }

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

            var capture = spriteBatch.Capture();

            spriteBatch.UseBlendState(BlendState.Additive);

            if (OldPositions != null)
            {
                for (int i = 0; i < OldPositions.Count; i++)
                {
                    float SclMod = MathHelper.Lerp(0f, 1f, (float)i / 12f);
                    spriteBatch.Draw(texture, OldPositions[i] - Main.screenPosition, null, color * 0.3f, 0f, origin, scale * SclMod, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, null, OpusColorUtils.Pastel(color, 0.5f), 0f, origin, scale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, null, OpusColorUtils.Pastel(color, 0.8f), 0f, origin, scale * 0.3f, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }

}