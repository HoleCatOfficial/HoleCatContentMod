using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DestroyerTest.Rarity
{
	public class RiftRarity1 : ModRarity
    {
        public override Color RarityColor => Opus.Sine(Color.Black, ColorLib.Rift, 0.1f); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Cyan; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }
	
}