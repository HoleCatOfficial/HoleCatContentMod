
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.Projectiles.Weapon.Summon.RiftWhip;
using DestroyerTest.Content.Tiles.RiftConfigurator;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftWhipT2 : ModItem
	{

		public override void SetDefaults() {
			Item.width = 38;
			Item.height = 34;
			Item.DefaultToWhip(ModContent.ProjectileType<RiftWhipT2Projectile>(), 70, 2, 14);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.autoReuse = true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Item_HeliciteCrystal>(8)
                .AddIngredient<RiftWhipT1>()
                .AddTile<Tile_RiftConfiguratorWeaponry>()
				.Register();
		}

		public override bool MeleePrefix() {
			return true;
		}
	}
}