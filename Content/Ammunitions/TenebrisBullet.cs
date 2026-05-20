
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Ammunitions
{
	public class TenebrisBullet : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 8;
			Item.height = 10;
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 0f;
			Item.value = Item.sellPrice(copper: 16);
			Item.shoot = ModContent.ProjectileType<TenebrisBulletProjectile>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 0.00000000005f * 0.01f;
            Item.ammo = AmmoID.Bullet;
		}

		public override void AddRecipes() {
			CreateRecipe(3)
				.AddIngredient(ItemID.ChlorophyteBullet)
                .AddIngredient<Tenebris>(2)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}