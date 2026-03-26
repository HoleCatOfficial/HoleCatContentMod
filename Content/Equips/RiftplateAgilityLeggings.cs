using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Legs)]
	public class RiftplateAgilityLeggings : ModItem
	{
		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1); 
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.defense = 25;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetModPlayer<RiftAgilityRunSpeeds>().Legs = true;
        }

		public override void AddRecipes() 
		{
			CreateRecipe()
                .AddIngredient<Living_Shadow>(30)
                .AddIngredient<Item_Riftplate>(10)
				.AddIngredient(ItemID.AnkletoftheWind)
                .AddTile<Tile_RiftConfiguratorArmory>()
                .Register();
		}
	}
}