using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class RiftCloudUpper : BasePRT
    {

        public int MaxLifetime => 120;
        public override void SetProperty()
        {

            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Lifetime = MaxLifetime;
            Scale = Main.rand.NextFloat(0.5f, 1.5f);
        }

        public override void AI()
        {
            if (LifetimeCompletion > 0.5f)
            {
                Scale *= 0.999f;

                if (Scale <= 0.00001f)
                {
                    Kill();
                }
            }
        }

        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TexValue, Frame, Color.Black);
            Texture2D LowerLayer = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/RiftCloudLower").Value;
            spriteBatch.Draw(LowerLayer, Frame, ColorLib.Rift);
            return false;
        }
    }

    
}