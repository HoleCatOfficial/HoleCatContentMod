using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
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
    public class SimpleExplosionParticle: BaseParticle<SimpleExplosionParticle>
    {
        public Vector2 position;
        public Vector2 velocity = Vector2.Zero;
        public Color color;
        BlendState blendState = BlendState.Additive;
        public float Opacity = 1.0f;

        public float scale = 0f;
        public float endScale = 1f;

        public float GrowRateStart = 0.1f;
        public float GrowRateEnd = 0.02f;

        float rotation;

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
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
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
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

       
        public override void Update(ref ParticleRendererSettings settings)
        {
            float Progress = scale / endScale;
            position += velocity;

            Opacity = 1f - MathHelper.Clamp((Progress - 0.5f) / 0.5f, 0f, 1f);

            if (GrowRateEnd == GrowRateStart)
            {
                scale += GrowRateStart;
            }
            else
            {
                scale += MathHelper.Lerp(GrowRateStart, GrowRateEnd, Progress);
            }

            if (scale > endScale)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        //Drawing

        public override PixelLayer PixelLayer => PixelLayer.AbovePlayer;

        public override bool DrawsPixelated => true;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleExplosionParticle").Value;

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
                Opus.StartSpriteBatchPixelated(spritebatch, blendState, SpriteSortMode.Immediate);
            }
            else
            {
                Opus.StartSpriteBatchPixelated(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
            }

            spritebatch.Draw(Tex, position - Main.screenPosition, null, c(), rotation, Tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }

    public class LerpingSimpleExplosionParticle : BaseParticle<LerpingSimpleExplosionParticle>
    {
        public Vector2 position;
        public Vector2 velocity = Vector2.Zero;
        Color color;
        BlendState blendState = BlendState.Additive;
        public float Opacity = 1.0f;

        public Color StartColor;
        public Color EndColor;

        public bool UsesColorMap = false;
        public Color[] ColorMap;

        public float scale = 0f;
        public float endScale = 1f;

        public float GrowRateStart = 0.1f;
        public float GrowRateEnd = 0.02f;

        float rotation;

        public void Prepare(Vector2 Position, Vector2 Velocity, Color StartColor, Color EndColor, float GrowSpeed, float EndScale, BlendState blendState)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.UsesColorMap = false;
            this.StartColor = StartColor;
            this.EndColor = EndColor;
            this.scale = 0f;
            this.endScale = EndScale;
            this.blendState = blendState;

            this.GrowRateStart = GrowSpeed;
            this.GrowRateEnd = GrowSpeed;
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public void Prepare(Vector2 Position, Vector2 Velocity, Color[] Colormap, float GrowSpeed, float EndScale, BlendState blendState)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.ColorMap = Colormap;
            this.blendState = blendState;
            this.UsesColorMap = true;
            this.endScale = EndScale;

            this.GrowRateStart = GrowSpeed;
            this.GrowRateEnd = GrowSpeed;
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }



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
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            float Progress = scale / endScale;
            position += velocity;

            Opacity = 1f - MathHelper.Clamp((Progress - 0.5f) / 0.5f, 0f, 1f);

            if (UsesColorMap)
            {
                color = OpusColorUtils.MultiLerp(Progress, ColorMap);
            }
            else
            {
                color = Color.Lerp(StartColor, EndColor, Progress);
            }

            if (GrowRateEnd == GrowRateStart)
            {
                scale += GrowRateStart;
            }
            else
            {
                scale += MathHelper.Lerp(GrowRateStart, GrowRateEnd, Progress);
            }

            if (scale > endScale)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        //Drawing

        public override PixelLayer PixelLayer => PixelLayer.AbovePlayer;

        public override bool DrawsPixelated => true;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleExplosionParticle").Value;

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
                Opus.StartSpriteBatchPixelated(spritebatch, blendState, SpriteSortMode.Immediate);
            }
            else
            {
                Opus.StartSpriteBatchPixelated(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
            }

            spritebatch.Draw(Tex, position - Main.screenPosition, null, c(), rotation, Tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }
}
