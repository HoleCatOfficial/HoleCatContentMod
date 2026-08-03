using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
 
using Microsoft.CodeAnalysis;
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
    public class DungeonSpiritParticle : BaseParticle<DungeonSpiritParticle>
    {
        public int Lifetime = 0;
        public int MaxLifetime = 80;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;

        public void Initialize(Vector2 Position, Vector2 Velocity, Color Color, float Scale)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.color = Color;
            this.scale = Scale;
        }

        //Since all of the particles deriving from this class use the same spritesheet format, the frame height and frame count are the same for all of them. 80x80 frame dimensions, 6 frames.
        public static int FrameHeight = 44;
        public static int FrameCount = 3;

        //Except for the frame tracker, used for iterating through the animation, though it isnt entirely useful, since the projectile just dies when the last frame is complete.
        public int CurrentFrame = 0;
        public void Anim()
        {
            if (Lifetime % 10 == 0)
            {
                CurrentFrame++;
                if (CurrentFrame > FrameCount)
                {
                    CurrentFrame = 0;
                }
            }
        }

        float Progress => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;
            position += velocity;

            if (Progress > 0.5f)
            {
                color *= 0.95f;
                scale *= 0.95f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            

            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/DungeonSpiritParticle").Value;

            int frameHeight = FrameHeight;
            Rectangle frame = new Rectangle(0, CurrentFrame * frameHeight, texture.Width, frameHeight);

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, 0f, origin, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}
