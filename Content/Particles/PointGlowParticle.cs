using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;

namespace DestroyerTest.Content.Particles
{
    /*
    public class PointGlowParticle : DTBaseParticle
    {
        public Vector2 Position;
        public int TimeLeft;
        public float MaxTime;
        public void Prepare(Vector2 SpawnPos, int MaxTime = 60)
        {
            Position = SpawnPos;
            this.MaxTime = MaxTime;
            TimeLeft = MaxTime;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (TimeLeft-- <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(DTAssetLib.PointGlow.Value, Position, null, Color.White, 0f, DTAssetLib.PointGlow.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
        
        
    }
    */
}
