
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Ammunitions
{
	public class EndlessTenebrisBullets : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() {
			Item.width = 30;
			Item.height = 54;
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = 1;
			Item.consumable = false;
			Item.knockBack = 0f;
			Item.value = Item.sellPrice(platinum: 9);
			Item.shoot = ModContent.ProjectileType<TenebrisBulletProjectile>(); // The projectile that weapons fire when using this item as ammunition.
			Item.shootSpeed = 2f;
			Item.ammo = AmmoID.Bullet;
		}
	}
}