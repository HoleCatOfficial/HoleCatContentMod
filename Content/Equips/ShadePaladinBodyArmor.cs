using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips
{

	[AutoloadEquip(EquipType.Body)]
    	public class ShadePaladinBodyArmor : ModItem
		{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 22; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 20;
		}

        public override void UpdateEquip(Player player) {

		}
	}
}