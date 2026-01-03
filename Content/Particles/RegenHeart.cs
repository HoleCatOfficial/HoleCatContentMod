using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Magic;
  
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    public class RegenHeart : BasePRT
    {
        public int MaxLifetime => 60;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
            Lifetime = MaxLifetime;
        }

        public override void AI()
        {
            Rotation += 0.005f * Velocity.X;

            if (LifetimeCompletion > 0.3f)
            {
                Color *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    } 
}