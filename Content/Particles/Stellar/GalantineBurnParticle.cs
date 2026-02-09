using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class GalantineBurnParticle : BasePRT
    {
        public int MaxLifetime => 60;
        public override void SetProperty()
        {
           
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            InitializeCaches(10);
        }

        public override void AI()
        {
            Scale *= 0.993f;
            Rotation += 0.1f * Velocity.X;

            Color = ColorLib.StellarFireGradient(LifetimeCompletion * 4f);

            Lighting.AddLight(Position, Color.ToVector3() * Scale);

            UpdatePositionCache(10);
            UpdateRotationCache(10);

        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 drawOrigin = new Vector2(TexValue.Width * 0.5f, TexValue.Height * 0.5f);
            for (int k = oldPositions.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (oldPositions[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Position.Y);
                Color color = Color * ((oldPositions.Length - k) / (float)oldPositions.Length);
                Main.EntitySpriteDraw(TexValue, drawPos, null, color, oldRotations[k], drawOrigin, Scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }

    
}