using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;

namespace DestroyerTest.Content.Equips.DemigodSet
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Legs)]
	public class DemigodBoots : ModItem
	{
		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 16; // The amount of defense the item will give when equipped
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.TitaniumBar,5)
                .AddIngredient<YellowCloth>(20)
                .AddIngredient<BrownCloth>(16)
                .AddIngredient<Shadowflame>(8)
				.AddTile(TileID.Anvils)
                .AddCondition(Condition.DownedSkeletron)
				.Register();
		}
	}
}