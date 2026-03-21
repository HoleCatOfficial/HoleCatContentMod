using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class StellarPointGlow : BasePRT
    {
        public int MaxLifetime => 100;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            Color = ColorLib.StellarFireGradient(LifetimeCompletion * 8f);
            if (LifetimeCompletion > 0.3f)
            {
                Color *= 0.8f;
                Scale *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color, Rotation,  TexValue.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, DTColorUtils.Pastel(Color, 0.85f), Rotation, TexValue.Size() / 2, Scale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
}