using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    public class SoulRarity : ModRarity
    {
        public override Color RarityColor => ColorLib.Soul3;

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ItemRarityID.Master;
            }

            return Type;
        }
    }

}