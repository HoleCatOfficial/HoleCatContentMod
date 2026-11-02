using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    public class EndemyRarity : ModRarity
    {
        public override Color RarityColor => new Color(0, 0, 0); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ItemRarityID.Master; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }

}