using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Legs)]
	public class TenebrousArchmagePants : ModItem
	{


		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 15;
		}

		public override void UpdateEquip(Player player) {
			player.GetCritChance(DamageClass.Magic) += 5f;
		}
		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}