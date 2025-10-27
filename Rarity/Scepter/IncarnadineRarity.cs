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
    /// The final rarity for the scepter class, covering most post-moonlord scepter gear.
    /// </summary>
	public class IncarnadineRarity : ModRarity
    {
        public override Color RarityColor => new Color(181, 0, 54); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ItemRarityID.Expert; // Upgrade to next rarity if necessary
            }
            if (offset < 0)
            {
                return ModContent.RarityType<CerisePinkRarity>();
            }
            return Type;
        }
    }
	
}