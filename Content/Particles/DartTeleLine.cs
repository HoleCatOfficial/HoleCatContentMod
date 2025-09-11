using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{

    internal class DartTeleLine : BasePRT
    {
        public int MaxLifetime => 120;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime; 
            Rotation = Rot; 
        }
        public float Rot = 0f;
        public override void AI()
        {
            Rotation = Rot;
            if (LifetimeCompletion > 0.3f)
            {
                Color *= 0.9f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    } 
}