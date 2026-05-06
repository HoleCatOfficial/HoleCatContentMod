using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class StellarPointGlow : PointGlow
    {
        public void Prepare(Vector2 Position, Vector2 Velocity)
        {
            position = Position;
            velocity = Velocity;
        }

        float Progress => (float)Lifetime / MaxLifetime;
        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;

            color = DTColorUtils.MultiLerp(Progress, ColorLib.StellarFireColormap);
            position += velocity;

            if (Progress > 0.5f)
            {
                color *= 0.95f;
                scale *= 0.95f;
            }

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
    }
 
}