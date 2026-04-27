using BreadLibrary.Core.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
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
    public class BlossomBeaterFire : BaseParticle<BlossomBeaterFire>
    {
        public int maxLifetime = 120;
        public int Lifetime = 0;
        public Color col;
        public float scale = 1f;
        private float InitScale = 1f;
        public Vector2 position;
        public float rotation;

        private float Opacity = 1f;

        public void Initiate(Vector2 Position, float Rotation, Color color, float Scale, int MaxLifetime)
        {
            this.position = Position;
            this.rotation = Rotation;
            this.scale = Scale;
            this.InitScale = Scale;
            this.maxLifetime = MaxLifetime;
            this.Lifetime = MaxLifetime;
            this.col = color;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime--;


            float progress = 1f - (float)Lifetime / (maxLifetime / 2f);
            Opacity = MathHelper.Lerp(1f, 0f, progress);
            //scale = MathHelper.Lerp(InitScale, InitScale * 0.5f, progress);
            

            if (Lifetime <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public Tuple<Texture2D, Rectangle, Vector2> GetTextureProperties()
        {
            Texture2D TexValue = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/BlossomBeaterFire").Value;
            Rectangle frameRect = new Rectangle(0, 0, TexValue.Width, TexValue.Height);

            Vector2 origin = new Vector2(TexValue.Width / 2f, TexValue.Height - 8);

            return new Tuple<Texture2D, Rectangle, Vector2>(TexValue, frameRect, origin);
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Opus.StartSpriteBatchWithBlending(spritebatch, BlendState.Additive, SpriteSortMode.Deferred);
            spritebatch.Draw(GetTextureProperties().Item1, position - Main.screenPosition, GetTextureProperties().Item2, col * Opacity, rotation, GetTextureProperties().Item3, new Vector2(scale, scale), SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }
}
