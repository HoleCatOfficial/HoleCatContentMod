using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class StellarFire : BasePRT
    {
        public int MaxLifetime => (int)ai[0];
        public int DrawMode => (int)ai[2];
        public bool LR;
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
            Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            Scale *= Main.rand.NextFloat(0.1f, 0.9f);
            LR = Main.rand.NextBool(2);
            ShouldKillWhenOffScreen = false;
        }
                
        //Since all of the particles deriving from this class use the same spritesheet format, the frame height and frame count are the same for all of them. 80x80 frame dimensions, 6 frames.
        public static int FrameHeight = 80;
        public static int FrameCount = 6;

        //Except for the frame tracker, used for iterating through the animation, though it isnt entirely useful, since the projectile just dies when the last frame is complete.
        public int CurrentFrame = 0;
        public void Anim()
        {
            Color = ColorLib.StellarFireGradient(LifetimeCompletion * 4f);
            ai[1]++;
            if (LR)
            {
                Rotation += 0.05f;
            }
            if (!LR)
            {
                Rotation -= 0.05f;
            }

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

            if (ai[1] % (MaxLifetime / FrameCount) == 0)
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

    public class StellarFire1 : StellarFire
    {

    }

    public class StellarFire2 : StellarFire
    {

    }
    public class StellarFire3 : StellarFire
    {

    }
    public class StellarFire4 : StellarFire
    {

    }
    public class StellarFire5 : StellarFire
    {

    }
    public class StellarFire6 : StellarFire
    {

    }
    public class StellarFire7 : StellarFire
    {

    }
    
    
}