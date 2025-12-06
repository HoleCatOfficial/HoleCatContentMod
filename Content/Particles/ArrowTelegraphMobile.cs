using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{

    internal class ArrowTelegraphMobile : BasePRT
    {
        public int MaxLifetime => 120;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime; 

        }
        public override void AI()
        {
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            if (LifetimeCompletion > 0.3f)
            {
                Color *= 0.9f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    } 
}