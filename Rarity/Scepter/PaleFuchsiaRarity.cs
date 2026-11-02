using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity.Scepter
{
    /// <summary>
    /// The major pre-hardmode rarity, spanning from King Slime's tier to the Wall of Flesh.
    /// </summary>
    public class PaleFuchsiaRarity : ModRarity
    {
        public override Color RarityColor => new Color(199, 67, 118); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<WineRarity>(); // Upgrade to next rarity if necessary
            }
            if (offset < 0)
            {
                return ModContent.RarityType<PearlRarity>();
            }
            return Type;
        }
    }

}