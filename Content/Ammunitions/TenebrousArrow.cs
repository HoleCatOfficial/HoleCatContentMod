
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;

namespace DestroyerTest.Content.Ammunitions
{
	public class TenebrousArrow : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 42;
			Item.height = 56;
			Item.damage = 28;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 2f;
			Item.value = Item.sellPrice(copper: 16);
			Item.shoot = ModContent.ProjectileType<TenebrisArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.
			Item.shootSpeed = 40f;
			Item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes() {
			CreateRecipe(2)
                .AddIngredient<Tenebris>()
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}