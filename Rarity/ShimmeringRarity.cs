using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DestroyerTest.Rarity
{
	public class ShimmeringRarity : ModRarity
    {
        public override Color RarityColor => ColorLib.TenebrisGradient; // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Lime; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }
	
}