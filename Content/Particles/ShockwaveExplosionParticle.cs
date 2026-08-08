using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
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
    public class ShockwaveExplosionParticle : BloomRingSharp
    {
        private BlendState blendState = BlendState.Additive;
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Texture2D value = DTAssetLib.ShockwaveExplosion.Value;
            if (blendState != BlendState.Additive)
            {
                Opus.StartSpriteBatchWithBlending(spritebatch, blendState, SpriteSortMode.Immediate);
            }
            else
            {
                Opus.StartSpriteBatchWithBlending(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
            }

            spritebatch.Draw(value, position - Main.screenPosition, null, c(), 0f, value.Size() / 2f, scale, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
            Color c()
            {
                if (blendState == BlendState.Additive)
                {
                    Color color = this.color;
                    color.A = 0;
                    return color * Opacity;
                }

                return this.color * Opacity;
            }
        }
    }
}
