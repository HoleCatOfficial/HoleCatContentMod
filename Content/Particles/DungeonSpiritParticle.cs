using InnoVault.PRT;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace DestroyerTest.Content.Particles
{
    public class DungeonSpiritParticle : BasePRT
    {
        public int MaxLifetime => 60;
        public int DrawMode => (int)ai[2];

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
            ShouldKillWhenOffScreen = false;
        }

        //Since all of the particles deriving from this class use the same spritesheet format, the frame height and frame count are the same for all of them. 80x80 frame dimensions, 6 frames.
        public static int FrameHeight = 44;
        public static int FrameCount = 3;

        //Except for the frame tracker, used for iterating through the animation, though it isnt entirely useful, since the projectile just dies when the last frame is complete.
        public int CurrentFrame = 0;
        public void Anim()
        {
            ai[1]++;

            if (ai[1] % 10 == 0)
            {
                CurrentFrame++;
                if (CurrentFrame > FrameCount)
                {
                    CurrentFrame = 0;
                }
            }
        }

        public override void AI()
        {
            Anim();
            Rotation = Velocity.ToRotation();
            if (CurrentFrame >= (FrameCount - 1))
            {
                Color *= 0.9f;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch)
        {

            int frameHeight = FrameHeight;
            Rectangle frame = new Rectangle(0, CurrentFrame * frameHeight, TexValue.Width, frameHeight);

            Vector2 origin = new Vector2(TexValue.Width / 2f, frameHeight / 2f);

            spriteBatch.Draw(
                TexValue,
                Position - Main.screenPosition,
                frame,
                Color,
                Rotation,
                origin,
                Scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }
}
