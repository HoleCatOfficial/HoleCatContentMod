using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity.Scepter
{
    /// <summary>
    /// The first scepter rarity tier. Everything pre-Boss.
    /// </summary>
	public class PearlRarity : ModRarity
    {
        public override Color RarityColor => new Color(253, 242, 255); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<PaleFuchsiaRarity>(); // Upgrade to next rarity if necessary
            }
            if (offset < 0)
            {
                return ItemRarityID.White;
            }
            return Type;
        }
    }

}