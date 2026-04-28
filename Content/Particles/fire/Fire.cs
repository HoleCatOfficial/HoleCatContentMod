using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.fire
{

    public enum FireDrawMode
    {
        AlphaBlend = 0,
        NonPremultiplied = 1,
        Opaque = 2,
        Additive = 3
    }

    public class Fire : BaseParticle<Fire>
    {
        int maxLifetime = 120;
        int Lifetime = 0;
        Vector2 position;
        Vector2 velocity;
        float rotation;
        Color col;
        float scale;
        float Opacity;

        int Variant = Main.rand.Next(1, 8);
        int NumFrames = 6;
        int frameInterval = 0;
        int frame = 0;

        //Spinning fire stuff
        bool isSpinningFire = false;
        float spinspeed = 1f;
        int spindirection = 0;

        int internalCounter = 0;

        FireDrawMode fireDrawMode = FireDrawMode.AlphaBlend;

        public void PrepareFire(Vector2 Position, Vector2 Velocity, float Rotation, Color color, float Scale, int MaxLifetime, FireDrawMode drawMode)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = Rotation;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.frameInterval = MaxLifetime / NumFrames;
            this.col = color;
            this.Opacity = 1f;

            this.isSpinningFire = false;
            fireDrawMode = drawMode;
        }

        public void PrepareFire(Vector2 Position, Vector2 Velocity, int SpinDirection, float SpinSpeed, Color color, float Scale, int MaxLifetime, FireDrawMode drawMode)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = 0f;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.frameInterval = MaxLifetime / NumFrames;
            this.col = color;
            this.Opacity = 1f;

            this.spindirection = SpinDirection;
            this.spinspeed = SpinSpeed;
            this.isSpinningFire = true;
            fireDrawMode = drawMode;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            internalCounter++;
            Lifetime--;

            position += velocity;

            if (isSpinningFire)
            {
                if (spindirection == 1)
                {
                    rotation += spinspeed * spindirection;
                }
                if (spindirection == -1)
                {
                    rotation -= spinspeed * spindirection;
                }
            }
            else
            {
                if (Main.rand.NextBool() && internalCounter % 30 == 0)
                {
                    rotation += 0.2f;
                }
                else
                {
                    rotation -= 0.2f;
                }
            }

            if (internalCounter % frameInterval == 0)
            {
                frame++;
            }

            if (Lifetime < (maxLifetime / 2))
            {
                Opacity *= 0.9f;
            }

            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public Tuple<Texture2D, Rectangle, Vector2> GetTextureProperties()
        {
            Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/fire/Fire{Variant}").Value;
            int frameHeight = 80;
            Rectangle frameRect = new Rectangle(0, frame * frameHeight, TexValue.Width, frameHeight);

            Vector2 origin = new Vector2(TexValue.Width / 2f, frameHeight / 2f);

            return new Tuple<Texture2D, Rectangle, Vector2>(TexValue, frameRect, origin);
        }

        public BlendState GetBlendState(FireDrawMode drawMode)
        {
            switch (drawMode)
            {
                case FireDrawMode.AlphaBlend:
                    {
                        return BlendState.AlphaBlend;
                    }
                case FireDrawMode.NonPremultiplied:
                    {
                        return BlendState.NonPremultiplied;
                    }
                case FireDrawMode.Opaque:
                    {
                        return BlendState.Opaque;
                    }
                case FireDrawMode.Additive:
                    {
                        return BlendState.Additive;
                    }
            }

            return BlendState.AlphaBlend;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Opus.StartSpriteBatchWithBlending(spritebatch, GetBlendState(fireDrawMode), SpriteSortMode.Deferred);
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col * Opacity, rotation, GetTextureProperties().Item3, scale, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }

    public class LerpingFire : BaseParticle<LerpingFire>
    {
        int maxLifetime = 120;
        int Lifetime = 0;
        Vector2 position;
        Vector2 velocity;
        float rotation;
        float Opacity;
        Color startcol;
        Color endcol;

        Color[] ColorMap;
        bool usesColorMap;

        Color col;
        float scale;

        int Variant = Main.rand.Next(1, 8);
        int NumFrames = 6;
        int frameInterval = 0;
        int frame = 0;

        //Spinning fire stuff
        bool isSpinningFire = false;
        float spinspeed = 1f;
        int spindirection = 0;

        int internalCounter = 0;

        FireDrawMode fireDrawMode = FireDrawMode.AlphaBlend;

        public void PrepareFire(Vector2 Position, Vector2 Velocity, float Rotation, Color startColor, Color endColor, float Scale, int MaxLifetime, FireDrawMode drawMode)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = Rotation;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.frameInterval = MaxLifetime / NumFrames;
            this.startcol = startColor;
            this.endcol = endColor;
            this.usesColorMap = false;
            this.Opacity = 1f;


            this.isSpinningFire = false;
            fireDrawMode = drawMode;
        }

        public void PrepareFire(Vector2 Position, Vector2 Velocity, int SpinDirection, float SpinSpeed, Color startColor, Color endColor, float Scale, int MaxLifetime, FireDrawMode drawMode)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.frameInterval = MaxLifetime / NumFrames;
            this.startcol = startColor;
            this.endcol = endColor;
            this.usesColorMap = false;
            this.Opacity = 1f;

            this.spindirection = SpinDirection;
            this.spinspeed = SpinSpeed;
            this.isSpinningFire = true;
            fireDrawMode = drawMode;
        }

        public void PrepareFire(Vector2 Position, Vector2 Velocity, float Rotation, Color[] colormap, float Scale, int MaxLifetime, FireDrawMode drawMode)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = Rotation;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.frameInterval = MaxLifetime / NumFrames;
            this.ColorMap = colormap;
            this.usesColorMap = true;
            this.Opacity = 1f;

            this.isSpinningFire = false;
            fireDrawMode = drawMode;
        }

        public void PrepareFire(Vector2 Position, Vector2 Velocity, int SpinDirection, float SpinSpeed, Color[] colormap, float Scale, int MaxLifetime, FireDrawMode drawMode)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.frameInterval = MaxLifetime / NumFrames;
            this.ColorMap = colormap;
            this.usesColorMap = true;
            this.Opacity = 1f;

            this.spindirection = SpinDirection;
            this.spinspeed = SpinSpeed;
            this.isSpinningFire = true;
            fireDrawMode = drawMode;
        }


        public override void Update(ref ParticleRendererSettings settings)
        {
            internalCounter++;
            Lifetime--;

            if (usesColorMap)
            {
                float Progress = (float)Lifetime / (float)maxLifetime;
                col = DTColorUtils.MultiLerp(Progress.Inverse(), ColorMap);
            }
            else
            {
                col = Color.Lerp(startcol, endcol, (float)(Lifetime / maxLifetime));
            }

            position += velocity;

            if (isSpinningFire)
            {
                if (spindirection == 1)
                {
                    rotation += spinspeed * spindirection;
                }
                if (spindirection == -1)
                {
                    rotation -= spinspeed * spindirection;
                }
            }
            else
            {
                if (Main.rand.NextBool() && internalCounter % 30 == 0)
                {
                    rotation += 0.2f;
                }
                else
                {
                    rotation -= 0.2f;
                }
            }

            if (internalCounter % frameInterval == 0)
            {
                frame++;
            }

            if (Lifetime < (maxLifetime / 2))
            {
                Opacity *= 0.9f;
            }

            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public Tuple<Texture2D, Rectangle, Vector2> GetTextureProperties()
        {
            Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/fire/Fire{Variant}").Value;
            int frameHeight = 80;
            Rectangle frameRect = new Rectangle(0, frame * frameHeight, TexValue.Width, frameHeight);

            Vector2 origin = new Vector2(TexValue.Width / 2f, frameHeight / 2f);

            return new Tuple<Texture2D, Rectangle, Vector2>(TexValue, frameRect, origin);
        }

        public BlendState GetBlendState(FireDrawMode drawMode)
        {
            switch (drawMode)
            {
                case FireDrawMode.AlphaBlend:
                    {
                        return BlendState.AlphaBlend;
                    }
                case FireDrawMode.NonPremultiplied:
                    {
                        return BlendState.NonPremultiplied;
                    }
                case FireDrawMode.Opaque:
                    {
                        return BlendState.Opaque;
                    }
                case FireDrawMode.Additive:
                    {
                        return BlendState.Additive;
                    }
            }

            return BlendState.AlphaBlend;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            
            Opus.StartSpriteBatchWithBlending(spritebatch, GetBlendState(fireDrawMode), SpriteSortMode.Deferred);
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col * Opacity, rotation, GetTextureProperties().Item3, scale, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
        }

        public override PixelLayer PixelLayer => PixelLayer.AboveTiles;
    }
}