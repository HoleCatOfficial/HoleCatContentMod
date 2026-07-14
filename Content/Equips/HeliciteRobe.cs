using rail;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Body)]
	public class HeliciteRobe : ModItem
	{
		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.defense = 35;
		}
        
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(60)
				.AddIngredient<Item_HeliciteCrystal>(30)
				.AddIngredient<Item_RiftClay>(22)
                .AddIngredient(ItemID.Silk, 15)
				.AddTile<Tile_RiftConfigurator>()
				.Register();
		}
	}
}