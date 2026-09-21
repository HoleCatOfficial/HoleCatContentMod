using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class FireRing : BloomRingSharp
    {
        BlendState blendState = BlendState.Additive;
        float r = 0f;


        public void Prepare(Vector2 Position, Vector2 Velocity, Color Color, float GrowSpeed, float EndScale, BlendState blendState)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = 0f;
            this.blendState = blendState;
            this.endScale = EndScale;

            this.GrowRateStart = GrowSpeed;
            this.GrowRateEnd = GrowSpeed;
            r = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public void Prepare(Vector2 Position, Vector2 Velocity, Color Color, float GrowSpeedStart, float GrowSpeedEnd, float EndScale, BlendState blendState)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = 0f;
            this.endScale = EndScale;
            this.blendState = blendState;

            this.GrowRateStart = GrowSpeedStart;
            this.GrowRateEnd = GrowSpeedEnd;
            r = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/FireRing").Value;

            Color c()
            {
                if (this.blendState == BlendState.Additive)
                {
                    return color with { A = 0 } * Opacity;
                }
                else
                {
                    return color * Opacity;
                }
            }

            if (blendState != BlendState.Additive)
            {
                Opus.StartSpriteBatchWithBlending(spritebatch, blendState, SpriteSortMode.Immediate);
            }
            else
            {
                Opus.StartSpriteBatchWithBlending(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
            }

            spritebatch.Draw(Tex, position - Main.screenPosition, null, c(), r, Tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }

    public class LerpingFireRing : LerpingBloomRingSharp
    {
        BlendState blendState = BlendState.Additive;
        Color color;
        float r = 0f;

        public void Prepare(Vector2 Position, Vector2 Velocity, Color StartColor, Color EndColor, float GrowSpeedStart, float GrowSpeedEnd, float EndScale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.StartColor = StartColor;
            this.EndColor = EndColor;
            this.scale = 0f;
            this.endScale = EndScale;

            this.GrowRateStart = GrowSpeedStart;
            this.GrowRateEnd = GrowSpeedEnd;
            r = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public void Prepare(Vector2 Position, Vector2 Velocity, Color[] Colormap, float GrowSpeedStart, float GrowSpeedEnd, float EndScale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.ColorMap = Colormap;
            this.UsesColorMap = true;
            this.endScale = EndScale;
            this.scale = 0f;

            this.GrowRateStart = GrowSpeedStart;
            this.GrowRateEnd = GrowSpeedEnd;
            r = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/FireRing").Value;

            Color c()
            {
                if (blendState == BlendState.Additive)
                {
                    return color with { A = 0 } * Opacity;
                }
                else
                {
                    return color * Opacity;
                }
            }

            if (blendState != BlendState.Additive)
            {
                Opus.StartSpriteBatchWithBlending(spritebatch, blendState, SpriteSortMode.Immediate);
            }
            else
            {
                Opus.StartSpriteBatchWithBlending(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
            }

            spritebatch.Draw(Tex, position - Main.screenPosition, null, c(), r, Tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }
}

