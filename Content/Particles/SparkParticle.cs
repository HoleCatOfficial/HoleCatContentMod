using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public enum SparkDrawMode
    {
        AlphaBlend = 0,
        NonPremultiplied = 1,
        Opaque = 2,
        Additive = 3
    }

    public class Spark : BaseParticle<Spark>
    {
        public int maxLifetime = 120;
        public int Lifetime = 0;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public Color col;
        public float scale;
        public float Opacity;
        public bool gravity;
        public bool[] TrackPlayer = new bool[Main.maxPlayers];

        public float Width = 1f;
        public float LengthMultiplier = 1f;
        public float _len = 1f;

        public int internalCounter = 0;

        public SparkDrawMode sparkDrawMode = SparkDrawMode.AlphaBlend;

        public void PrepareSpark(Vector2 Position, Vector2 Velocity, float Rotation, Color color, float Scale, bool Gravity, int MaxLifetime, SparkDrawMode drawMode, float lengthMultiplier = 1f)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = Rotation + MathHelper.PiOver2;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.col = color;
            this.Opacity = 1f;
            this.gravity = Gravity;
            this.LengthMultiplier = _len = lengthMultiplier;

            sparkDrawMode = drawMode;
        }



        public override void Update(ref ParticleRendererSettings settings)
        {
            internalCounter++;
            Lifetime--;

            position += velocity;

            rotation = velocity.ToRotation();

            if (gravity)
            {
                velocity.Y += 0.8f;
            }

            for (int i = 1; i < Main.maxPlayers; i++)
            {
                if (TrackPlayer[i])
                {
                    position += Main.player[i].velocity;
                }
            }

            if (Lifetime < maxLifetime / 2)
            {
                float progress = 1f - (float)Lifetime / (maxLifetime / 2f);
                Width = MathHelper.Lerp(1f, 0f, progress);

      
            }

            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public Tuple<Texture2D, Rectangle, Vector2> GetTextureProperties()
        {
            //Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/SparkParticle").Value;

            Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Extras/MiscSparkle2").Value;
            Rectangle frameRect = new Rectangle(0, 0, TexValue.Width, TexValue.Height);

            Vector2 origin = new Vector2(TexValue.Width / 2f, TexValue.Height / 2f);

            return new Tuple<Texture2D, Rectangle, Vector2>(TexValue, frameRect, origin);
        }

        public BlendState GetBlendState(SparkDrawMode drawMode)
        {
            switch (drawMode)
            {
                case SparkDrawMode.AlphaBlend:
                    {
                        return BlendState.AlphaBlend;
                    }
                case SparkDrawMode.NonPremultiplied:
                    {
                        return BlendState.NonPremultiplied;
                    }
                case SparkDrawMode.Opaque:
                    {
                        return BlendState.Opaque;
                    }
                case SparkDrawMode.Additive:
                    {
                        return BlendState.Additive;
                    }
            }

            return BlendState.AlphaBlend;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Opus.StartSpriteBatchWithBlending(spritebatch, GetBlendState(sparkDrawMode), SpriteSortMode.Deferred);
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col * Opacity, rotation, GetTextureProperties().Item3, new Vector2(scale * LengthMultiplier, scale * Width) * 0.1f, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }

    public class LerpingSpark : BaseParticle<LerpingSpark>
    {
        public int maxLifetime = 120;
        public int Lifetime = 0;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float Opacity;
        public Color startcol;
        public Color endcol;
        public bool[] TrackPlayer = new bool[Main.maxPlayers];

        public Color[] ColorMap;
        public bool usesColorMap;

        public Color col;
        public float scale;
        public bool gravity;

        public float Width = 1f;
        public float LengthMultiplier = 1f;

        public int internalCounter = 0;

        public SparkDrawMode sparkDrawMode = SparkDrawMode.AlphaBlend;

        public void PrepareSpark(Vector2 Position, Vector2 Velocity, float Rotation, Color startColor, Color endColor, float Scale, bool Gravity, int MaxLifetime, SparkDrawMode drawMode, float lengthMultiplier = 1f)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = Rotation + MathHelper.PiOver2;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.startcol = startColor;
            this.endcol = endColor;
            this.usesColorMap = false;
            this.Opacity = 1f;
            this.gravity = Gravity;
            this.LengthMultiplier = lengthMultiplier;

            sparkDrawMode = drawMode;
        }

        public void PrepareSpark(Vector2 Position, Vector2 Velocity, float Rotation, Color[] Colormap, float Scale, bool Gravity, int MaxLifetime, SparkDrawMode drawMode, float lengthMultiplier = 1f)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.rotation = Rotation;
            this.scale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.ColorMap = Colormap;
            this.usesColorMap = true;
            this.usesColorMap = false;
            this.Opacity = 1f;
            this.gravity = Gravity;
            this.LengthMultiplier = lengthMultiplier;


            sparkDrawMode = drawMode;
        }


        public override void Update(ref ParticleRendererSettings settings)
        {
            internalCounter++;
            Lifetime--;

            if (usesColorMap)
            {
                col = DTColorUtils.MultiLerp((float)(Lifetime / maxLifetime), ColorMap);
            }
            else
            {
                col = Color.Lerp(startcol, endcol, (float)(Lifetime / maxLifetime));
            }

            position += velocity;

            for (int i = 1; i < Main.maxPlayers; i++)
            {
                if (TrackPlayer[i])
                {
                    position += Main.player[i].velocity;
                }
            }


            rotation = velocity.ToRotation();

            if (gravity)
            {
                velocity.Y += 0.8f;
            }

            if (Lifetime < maxLifetime / 2)
            {
                float progress = 1f - (float)Lifetime / (maxLifetime / 2f);
                Width = MathHelper.Lerp(1f, 0f, progress);
            }

            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public Tuple<Texture2D, Rectangle, Vector2> GetTextureProperties()
        {
            //Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/SparkParticle").Value;

            Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Extras/MiscSparkle2").Value;
            Rectangle frameRect = new Rectangle(0, 0, TexValue.Width, TexValue.Height);

            Vector2 origin = new Vector2(TexValue.Width / 2f, TexValue.Height / 2f);

            return new Tuple<Texture2D, Rectangle, Vector2>(TexValue, frameRect, origin);
        }

        public BlendState GetBlendState(SparkDrawMode drawMode)
        {
            switch (drawMode)
            {
                case SparkDrawMode.AlphaBlend:
                    {
                        return BlendState.AlphaBlend;
                    }
                case SparkDrawMode.NonPremultiplied:
                    {
                        return BlendState.NonPremultiplied;
                    }
                case SparkDrawMode.Opaque:
                    {
                        return BlendState.Opaque;
                    }
                case SparkDrawMode.Additive:
                    {
                        return BlendState.Additive;
                    }
            }

            return BlendState.AlphaBlend;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {

            Opus.StartSpriteBatchWithBlending(spritebatch, GetBlendState(sparkDrawMode), SpriteSortMode.Deferred);
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col * Opacity, rotation, GetTextureProperties().Item3, new Vector2(scale * LengthMultiplier, scale * Width) * 0.1f, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveTiles;
    }

    public class HeatseekerSilohSpark : Spark
    {
        public override void Update(ref ParticleRendererSettings settings)
        {
            internalCounter++;
            Lifetime--;

            position += velocity;

            rotation = velocity.ToRotation();
            velocity *= 0.97f;

            if (Lifetime < maxLifetime / 2)
            {
                float progress = 1f - (float)Lifetime / (maxLifetime / 2f);
                Width = MathHelper.Lerp(1f, 0f, progress);
                LengthMultiplier = MathHelper.Lerp(_len, 0f, progress);
            }

            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
    }

    public class WitheringSpark : Spark
    {
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col with { A = 0 } * Opacity, rotation, GetTextureProperties().Item3, new Vector2(scale * Width * LengthMultiplier, scale) * 0.1f, SpriteEffects.None, 0f);
            
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, Color.Black * Opacity, rotation, GetTextureProperties().Item3, new Vector2(scale * 0.5f * Width * LengthMultiplier, (scale) * 0.7f) * 0.1f, SpriteEffects.None, 0f);
        }
    }


    /*
        public class SparkParticle : BasePRT
        {
            public int MaxLifetime => 1200;
            public int DrawMode => (int)ai[1];
            public override void SetProperty()
            {
                if (DrawMode == 0)
                {
                    PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
                }
                if (DrawMode == 1)
                {
                    PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
                }
                if (DrawMode == 2)
                {
                    PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
                }
                Lifetime = MaxLifetime;
                LengthScale = 1 + 0.1f * Velocity.Length();
            }
            float LengthScale = 1;
            float WidthScale = 1;
            public override void AI()
            {
                Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
                Velocity.Y += 0.3f;
                WidthScale *= 0.9f;
                LengthScale *= 0.95f;

                if (WidthScale <= 0.0001f)
                {
                    Kill();
                }
            }

            // Override this drawing function. If you want to customize the drawing, return false here,
            // and the default drawing will not be applied.
            public override bool PreDraw(SpriteBatch spriteBatch)
            {
                Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, new Vector2(WidthScale, LengthScale), SpriteEffects.None, 0);
                return false;
            }
        }

        public class SparkParticleNoGravity : BasePRT
        {
            public int MaxLifetime => 1200;
            public int DrawMode => (int)ai[1];
            public override void SetProperty()
            {
                if (DrawMode == 0)
                {
                    PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
                }
                if (DrawMode == 1)
                {
                    PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
                }
                if (DrawMode == 2)
                {
                    PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
                }
                Lifetime = MaxLifetime;
                LengthScale = 1 + 0.1f * Velocity.Length();
            }
            float LengthScale = 1;
            float WidthScale = 1;
            public override void AI()
            {
                Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
                WidthScale *= 0.90f;
                LengthScale *= 0.95f;

                if (WidthScale <= 0.0001f)
                {
                    Kill();
                }
            }

            // Override this drawing function. If you want to customize the drawing, return false here,
            // and the default drawing will not be applied.
            public override bool PreDraw(SpriteBatch spriteBatch)
            {
                Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, new Vector2(WidthScale, LengthScale), SpriteEffects.None, 0);
                return false;
            }
        }


        public class SparkParticlePlayerLock : BasePRT
        {
            public int MaxLifetime => 1200;
            public int DrawMode => (int)ai[1];
            public int Owner => (int)ai[2];
            public override void SetProperty()
            {
                if (DrawMode == 0)
                {
                    PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
                }
                if (DrawMode == 1)
                {
                    PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
                }
                if (DrawMode == 2)
                {
                    PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
                }
                Lifetime = MaxLifetime;
                LengthScale = 1 + 0.1f * Velocity.Length();
            }
            float LengthScale = 1;
            float WidthScale = 1;
            public override void AI()
            {
                Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
                WidthScale *= 0.90f;
                LengthScale *= 0.95f;

                Player o = Main.player[Owner];

                Position += o.velocity;

                if (WidthScale <= 0.0001f)
                {
                    Kill();
                }
            }

            // Override this drawing function. If you want to customize the drawing, return false here,
            // and the default drawing will not be applied.
            public override bool PreDraw(SpriteBatch spriteBatch)
            {
                Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, new Vector2(WidthScale, LengthScale), SpriteEffects.None, 0);
                return false;
            }
        }

        public class WitheringSpark : BasePRT
        {
            public int MaxLifetime => 1200;
            public int DrawMode => (int)ai[1];
            public override void SetProperty()
            {
                if (DrawMode == 0)
                {
                    PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
                }
                if (DrawMode == 1)
                {
                    PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
                }
                if (DrawMode == 2)
                {
                    PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
                }
                Lifetime = MaxLifetime;
                LengthScale = 1 + 0.1f * Velocity.Length();
            }
            float LengthScale = 1;
            float WidthScale = 1;
            public override void AI()
            {
                Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
                WidthScale *= 0.90f;
                LengthScale *= 0.95f;

                if (WidthScale <= 0.0001f)
                {
                    Kill();
                }
            }

            // Override this drawing function. If you want to customize the drawing, return false here,
            // and the default drawing will not be applied.
            public override bool PreDraw(SpriteBatch spriteBatch)
            {
                Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, new Vector2(WidthScale, LengthScale), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color.Black, Rotation, TexValue.Size() / 2, new Vector2(WidthScale * 0.4f, LengthScale * 0.4f), SpriteEffects.None, 0);
                return false;
            }
        }

        */


}