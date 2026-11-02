using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    public class CelestialRarity : ModRarity
    {
        public override Color RarityColor => ColorLib.CelestialGradient;

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }

}