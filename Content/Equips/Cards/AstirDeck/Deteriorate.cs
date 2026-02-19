
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
	public class Deteriorate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 1;
			Item.accessory = true;
            Item.rare = ItemRarityID.White;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

		}
    }
    
}