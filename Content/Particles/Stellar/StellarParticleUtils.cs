using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Renderers;

namespace DestroyerTest.Content.Particles.Stellar
{
    public static class StellarParticleUtils
    {
        public static Color[] Colormap = ColorLib.StellarFireColormap;

        public static void BloomRing(Vector2 position, float scale, ParticleRenderer Layer)
        {
            LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
            Ring.Prepare(position, Vector2.Zero, Colormap, 0.1f, 0.02f, scale); 
            Layer.Add(Ring);
        }

        public static void FlatStar(Vector2 position, float scale, ParticleRenderer Layer)
        {
            FlatStarStellar star = new FlatStarStellar();
            star.Prepare(position, Vector2.Zero, scale);
            Layer.Add(star);
        }
    }
}
