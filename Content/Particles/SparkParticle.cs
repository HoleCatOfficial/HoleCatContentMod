using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class SparkParticle : BasePRT
    {
        public int MaxLifetime => 1200;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            LengthScale = 1 + 0.1f * Velocity.Length();
        }
        float LengthScale = 1;
        float WidthScale = 1;
        public override void AI()
        {
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            Velocity.Y += 0.3f;
            WidthScale *= 0.95f;
            LengthScale *= 0.995f;
            
            if (WidthScale <= 0.0001f)
            {
                Kill();
            }
        }

        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, new Vector2(WidthScale, LengthScale), SpriteEffects.None, 0);
            return false;
        }
    }

    internal class SparkParticleNoGravity : BasePRT
    {
        public int MaxLifetime => 1200;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            LengthScale = 1 + 0.1f * Velocity.Length();
        }
        float LengthScale = 1;
        float WidthScale = 1;
        public override void AI()
        {
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            WidthScale *= 0.95f;
            LengthScale *= 0.995f;
            
            if (WidthScale <= 0.0001f)
            {
                Kill();
            }
        }

        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Main.EntitySpriteDraw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, new Vector2(WidthScale, LengthScale), SpriteEffects.None, 0);
            return false;
        }
    }

    
}