using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Resources.Cloths;

namespace DestroyerTest.Content.Equips
{

	[AutoloadEquip(EquipType.Legs)]
	public class HoleCatLegs : ModItem
	{
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Expert;
            Item.defense = 12;
		}

        public override void UpdateEquip(Player player)
        {
            
		}
	}
}