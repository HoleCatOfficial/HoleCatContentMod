using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using DestroyerTest.Content.Tiles;

namespace DestroyerTest.Rarity.Scepter
{
    /// <summary>
    /// The fourth rarity, spanning from Plantera to the Lunar Event.
    /// </summary>
	public class CerisePinkRarity : ModRarity
    {
        public override Color RarityColor => new Color(236, 59, 131); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<IncarnadineRarity>(); // Upgrade to next rarity if necessary
            }
            if (offset < 0)
            {
                return ModContent.RarityType<WineRarity>();
            }
            return Type;
        }
    }
	
}