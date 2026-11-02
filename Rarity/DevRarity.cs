using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    /// <summary>
    /// D3 - Dev, Donor, Dedicatee.
    /// </summary>
    public class DevRarity : ModRarity
    {
        public override Color RarityColor => Main.DiscoColor;

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