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

namespace DestroyerTest.Content.Particles.Comaceratic
{
    public class ComaceraticParticle : BasePRT
    {
        public int MaxLifetime => 75;
      
        public static int FrameHeight = 700;
        public static int FrameCount = 3;

        public int CurrentFrame = 0;

        public override void SetProperty()
        {

            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            CurrentFrame = Main.rand.Next(3);
            
        }

        public override void AI()
        {

            Velocity *= 0.96f;
            Rotation += 0.06f * (Velocity.X > 0 ? 1 : -1);

            if (LifetimeCompletion > 0.6f)
            {
                Color *= 0.9f;
                Scale *= 0.9f;
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
