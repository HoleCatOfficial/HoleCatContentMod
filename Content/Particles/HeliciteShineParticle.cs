using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Magic;
  
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class HeliciteShineParticle : BasePRT
    {
        public int MaxLifetime => 120;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            Color = Color.White;
        }

        public float Scale1 = 0f;
        public float Scale2 = 0f;

        public override void AI()
        {
            Scale1 = Opus.Sine(0.5f, 0.8f, 0.01f);
            Scale2 = Opus.Sine(0.1f, 0.5f, 0.2f);

            if (LifetimeCompletion > 0.5f)
            {
                Opacity += 0.01f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Main.EntitySpriteDraw(DTAssetLib.PointGlow.Value, Position - Main.screenPosition, null, ColorLib.Rift * Opacity.Inverse(), 0f, DTAssetLib.PointGlow.Value.Size() / 2f, new Vector2(Scale1, Scale1), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Sparkle(5).Value, Position - Main.screenPosition, null, Color.White * Opacity.Inverse(), 0f, DTAssetLib.Sparkle(5).Value.Size() / 2f, new Vector2(Scale2, Scale2), SpriteEffects.None);
            return true;
        }
    } 
}