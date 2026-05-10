using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Equips.PotionFlowers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.Particles.PotionFlowers
{
    public class LilliesOfImmortalityParticle : BaseParticle<LilliesOfImmortalityParticle>
    {
        public Vector2 position;
        public float scale;
        public int Lifetime = 0;
        public int MaxLifetime = 240;

        float Opacity = 1.0f;
        public void Spawn(Vector2 Position, float Scale = 1f)
        {
            this.position = Position;
            this.scale = Scale;
        }

        SpriteEffects Fx = SpriteEffects.None;
        float WidthModifier = 1f;
        bool HasFlipped = false;
        public override void Update(ref ParticleRendererSettings settings)
        {
            position += new Vector2(0, -1.2f);


            WidthModifier = Opus.Sine(1f, 0f, 0.2f);

            if (WidthModifier < 0.02f)
            {
                if (Fx == SpriteEffects.None && !HasFlipped)
                {
                    Fx = SpriteEffects.FlipHorizontally;
                    HasFlipped = true;
                }
                if (Fx == SpriteEffects.FlipHorizontally && !HasFlipped)
                {
                    Fx = SpriteEffects.None;
                    HasFlipped = true;
                }
                
            }
            if (WidthModifier > 0.5f)
            {
                HasFlipped = false;
            }

            Lifetime++;

            Opacity = MathHelper.Lerp(1f, 0f, ((float)Lifetime / (float)MaxLifetime));

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Texture2D Texture = TextureAssets.Item[ModContent.ItemType<LilliesOfImmortality>()].Value;
            
            spritebatch.Draw(Texture, position - Main.screenPosition, null, Color.White * Opacity, 0f, Texture.Size() / 2, new Vector2(scale * WidthModifier, scale), Fx, 0f);
        }

    }
}
