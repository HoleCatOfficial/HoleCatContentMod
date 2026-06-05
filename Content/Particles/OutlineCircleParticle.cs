using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public  class OutlineCircleParticle : BaseParticle<OutlineCircleParticle>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 300;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;

        public void Create(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            position += velocity;

            scale *= 0.97f;

            if (scale < 0.01f)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Texture2D tinybloom = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/TinyBloom").Value;

            spritebatch.UseBlendState(BlendState.AlphaBlend);
            spritebatch.Draw(DTAssetLib.PointGlowPreMultiplied.Value, position - Main.screenPosition, null, color, 0f, DTAssetLib.PointGlow.Size() / 2f, scale * 1.5f, SpriteEffects.None, 0f);
            spritebatch.Draw(tinybloom, position - Main.screenPosition, null, Color.Black, 0f, tinybloom.Size() / 2f, scale, SpriteEffects.None, 0f);
            spritebatch.ResetToDefault();
        }

    }
}
