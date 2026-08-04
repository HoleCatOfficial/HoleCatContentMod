using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.Particles
{
    public class TintableSmoke : BaseParticle<TintableSmoke>
    {
        public Vector2 position;
        public Vector2 velocity = Vector2.Zero;

        public float Opacity = 1.0f;
        public float _Opacity;
        float Rotation = 0f;
        public float scale = 0f;
        int LifeTime = 0;
        int MaxLifetime = 120;
        Color color;
        int variant = 0;

        public bool Flat = false;
        Asset<Texture2D> Tex => Flat ? ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/TintableSmokeFlat") : ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/TintableSmoke");
        BlendState internalBlending = BlendState.AlphaBlend;

        public void Create(Vector2 Position, Vector2 Velocity, Color Color, float Opcaity, float Scale, int LifeTime, PixelLayer Layer, bool Flat = false)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.scale = Scale;
            this.color = Color;
            this._Opacity = Opcaity;
            this.MaxLifetime = LifeTime;
            this.PixelLayer = Layer;
            this.Flat = Flat;
            this.variant = Main.rand.Next(3);
        }

        public void CreateWithBlending(Vector2 Position, Vector2 Velocity, Color Color, float Opacoty, float Scale, int LifeTime, PixelLayer Layer, BlendState blending, bool Flat = false)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.scale = Scale;
            this.color = Color;
            this._Opacity = Opacoty;
            this.MaxLifetime = LifeTime;
            this.PixelLayer = Layer;
            this.Flat = Flat;
            this.internalBlending = blending;
            this.variant = Main.rand.Next(3);
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            HasExplicitPixelLayer = false;
            LifeTime++;
            float Progress = (float)LifeTime / (float)MaxLifetime;

            position += velocity;
            Rotation += Main.rand.NextFloat(0.1f) * Math.Sign(velocity.X);
            Opacity = MathHelper.Lerp(_Opacity, 0f, Progress);

            if (Progress >= 1)
            {
                ShouldBeRemovedFromRenderer = true;
            }

        }

        Vector2 Dimensions = new Vector2(40, 40);

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Rectangle Frame = new Rectangle(0, 40 * variant, (int)Dimensions.X, (int)Dimensions.Y);
            spritebatch.UseBlendState(internalBlending);
            spritebatch.Draw(Tex.Value, position - Main.screenPosition, Frame, color * Opacity, Rotation, Frame.Size() / 2, scale, SpriteEffects.None, 0f);
            spritebatch.ResetToDefault();
        }
    }
}
