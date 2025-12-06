
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles.Weapon.Summon;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftWhipT2 : ModItem
	{

		public override void SetDefaults() {
			Item.width = 38;
			Item.height = 34;
			Item.DefaultToWhip(ModContent.ProjectileType<RiftWhipT2Projectile>(), 70, 2, 4);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.channel = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Item_HeliciteCrystal>(8)
				.AddTile<Tile_RiftCrucible>()
				.Register();
		}

		// Makes the whip receive melee prefixes
		public override bool MeleePrefix() {
			return true;
		}
	}
}