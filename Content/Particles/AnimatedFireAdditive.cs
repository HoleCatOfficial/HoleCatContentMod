using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class AnimatedFireAdditive_Base : BasePRT
    {
        public int MaxLifetime => 60;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            Scale *= Main.rand.NextFloat(0.1f, 0.9f);
        }
                
        //Since all of the particles deriving from this class use the same spritesheet format, the frame height and frame count are the same for all of them. 80x80 frame dimensions, 6 frames.
        public static int FrameHeight = 80;
        public static int FrameCount = 6;

        //Except for the frame tracker, used for iterating through the animation, though it isnt entirely useful, since the projectile just dies when the last frame is complete.
        public int CurrentFrame = 0;
        public void Anim()
        {
            ai[0]++;

            if (ai[0] % (MaxLifetime / FrameCount) == 0)
            {
                CurrentFrame++;
                if (CurrentFrame >= FrameCount)
                {
                    Lifetime = 0;
                }
            }
        }

        public override void AI()
        {
            Anim();
            if (LifetimeCompletion > 0.85f)
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

    internal class AnimatedFireAdditive1 : AnimatedFireAdditive_Base
    {

    }

    internal class AnimatedFireAdditive2 : AnimatedFireAdditive_Base
    {

    }
    internal class AnimatedFireAdditive3 : AnimatedFireAdditive_Base
    {

    }
    internal class AnimatedFireAdditive4 : AnimatedFireAdditive_Base
    {

    }
    internal class AnimatedFireAdditive5 : AnimatedFireAdditive_Base
    {

    }
    internal class AnimatedFireAdditive6 : AnimatedFireAdditive_Base
    {

    }
    internal class AnimatedFireAdditive7 : AnimatedFireAdditive_Base
    {

    }
    
    
}