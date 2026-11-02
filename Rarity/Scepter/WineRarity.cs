using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity.Scepter
{
    /// <summary>
    /// The third rarity, spanning from pre-mech to plantera.
    /// </summary>
	public class WineRarity : ModRarity
    {
        public override Color RarityColor => new Color(157, 7, 89); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<CerisePinkRarity>(); // Upgrade to next rarity if necessary
            }
            if (offset < 0)
            {
                return ModContent.RarityType<PaleFuchsiaRarity>();
            }
            return Type;
        }
    }

}