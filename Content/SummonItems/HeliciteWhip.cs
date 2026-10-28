
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Projectiles.Weapon.Summon.HeliciteWhip;
using DestroyerTest.Content.RiftArsenal;

namespace DestroyerTest.Content.SummonItems
{
	public class HeliciteWhip : ModItem
	{

		public override void SetDefaults() {
			Item.width = 38;
			Item.height = 34;
			Item.DefaultToWhip(ModContent.ProjectileType<HeliciteWhipProjectile>(), 120, 2, 14);
			Item.rare = ModContent.RarityType<HeliciteRarity>();
			Item.autoReuse = true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Item_HeliciteCrystal>(8)
                .AddIngredient<RiftWhip>()
                .AddTile<Tile_RiftConfigurator>()
				.Register();
		}

		public override bool MeleePrefix() {
			return true;
		}
	}
}