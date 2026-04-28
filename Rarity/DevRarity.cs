using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ID;

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