using System.Collections.Generic;
using InnoVault.PRT;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class StellarParticleIndex
    {
        public static int ConstitutionParticle = PRTLoader.GetParticleID<ConstitutionParticle>();
        public static int GalantineBurnParticle = PRTLoader.GetParticleID<GalantineBurnParticle>();
        public static int FlatStar = PRTLoader.GetParticleID<FlatStarStellar>();
        public static List<int> StellarFire = new List<int>
        {
            PRTLoader.GetParticleID<StellarFire1>(),
            PRTLoader.GetParticleID<StellarFire2>(),
            PRTLoader.GetParticleID<StellarFire3>(),
            PRTLoader.GetParticleID<StellarFire4>(),
            PRTLoader.GetParticleID<StellarFire5>(),
            PRTLoader.GetParticleID<StellarFire6>(),
            PRTLoader.GetParticleID<StellarFire7>()
        };

        public static int BloomRing = PRTLoader.GetParticleID<BloomRingStellar>();
        public static int BloomRingSharp = PRTLoader.GetParticleID<BloomRingSharpStellar>();
        public static int PointGlow = PRTLoader.GetParticleID<StellarPointGlow>();
    }
}