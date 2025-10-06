
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;

namespace DestroyerTest.Content.Ammunitions
{
	public class RiftRound : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 8;
			Item.height = 8;
			Item.damage = 15;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 0f;
			Item.value = Item.sellPrice(copper: 16);
			Item.shoot = ModContent.ProjectileType<RiftRoundProjectile>(); // The projectile that weapons fire when using this item as ammunition.
			Item.shootSpeed = 5f;
			Item.ammo = AmmoID.Bullet;
		}

		public override void AddRecipes() {
			CreateRecipe(2)
				.AddIngredient(ItemID.MusketBall)
                .AddIngredient<Living_Shadow>(2)
				.AddTile<Tile_RiftConfiguratorWeaponry>()
				.Register();
		}
	}
}