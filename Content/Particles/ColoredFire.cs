using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class ColoredFireBase : BasePRT
    {
        public int MaxLifetime => 20;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            Scale *= Main.rand.NextFloat(0.1f, 0.9f);
        }

        public override void AI()
        {
            

            if (LifetimeCompletion > 0.4f)
            {
                Color *= 0.9f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }

    internal class ColoredFire1 : ColoredFireBase
    {

    }

    internal class ColoredFire2 : ColoredFireBase
    {

    }
    internal class ColoredFire3 : ColoredFireBase
    {

    }
    internal class ColoredFire4 : ColoredFireBase
    {

    }
    internal class ColoredFire5 : ColoredFireBase
    {

    }
    internal class ColoredFire6 : ColoredFireBase
    {

    }
    internal class ColoredFire7 : ColoredFireBase
    {

    }
    
    
}