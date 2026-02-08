using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class WingDisableParticle : BasePRT
    {
        public int MaxLifetime => 180;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Lifetime = MaxLifetime;
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            Scale += 0.25f;
            Vector2 ScreenCenter = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            Position = ScreenCenter;
            if (LifetimeCompletion > 0.7f)
            {
                Color *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return true;
        }
    }
}