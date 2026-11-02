using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    public class LifeEchoRarity : ModRarity
    {
        public override Color RarityColor => new Color(20, 20, 25); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ItemRarityID.Green; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }

}