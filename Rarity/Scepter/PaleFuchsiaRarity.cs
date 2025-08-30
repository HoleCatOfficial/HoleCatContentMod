using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using DestroyerTest.Content.Tiles;

namespace DestroyerTest.Rarity
{
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