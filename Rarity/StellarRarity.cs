using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    public class StellarRarity : ModRarity
    {
        public override Color RarityColor => ColorLib.StellarFireGradientLooping();

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ItemRarityID.Red;
            }

            return Type;
        }
    }

}