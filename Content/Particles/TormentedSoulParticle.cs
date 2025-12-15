using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{

    internal class TormentedSoulParticle : BasePRT
    {

        public int MaxLifetime => 120;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Lifetime = MaxLifetime;
            InitializePositionCache(10);
            Color = Color.White;
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            Rotation = Velocity.ToRotation() - MathHelper.PiOver2;
            UpdatePositionCache(oldPositions.Length);

            if (LifetimeCompletion > 0.5f)
            {
                Color *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 drawOrigin = new Vector2(TexValue.Width * 0.5f, TexValue.Height * 0.5f);
            for (int k = oldPositions.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (oldPositions[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Position.Y);
                Color color = Color.Purple * ((oldPositions.Length - k) / (float)oldPositions.Length);
                Main.EntitySpriteDraw(TexValue, drawPos, null, color, Rotation, drawOrigin, Scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    
    internal class TormentedSoulParticle2 : BasePRT
    {
      
        public int MaxLifetime => 120;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Lifetime = MaxLifetime;
            InitializePositionCache(10);
            Color = Color.White;
        }

        public override void AI()
        {
            Rotation = Velocity.ToRotation() - MathHelper.PiOver2;
            Velocity.Y -= 0.5f;
            UpdatePositionCache(oldPositions.Length);

            if (LifetimeCompletion > 0.5f)
            {
                Color *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 drawOrigin = new Vector2(TexValue.Width * 0.5f, TexValue.Height * 0.5f);
            for (int k = oldPositions.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (oldPositions[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Position.Y);
                Color color = Color.Purple * ((oldPositions.Length - k) / (float)oldPositions.Length);
                Main.EntitySpriteDraw(TexValue, drawPos, null, color, Rotation, drawOrigin, Scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}