
using rail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Ammunitions
{
	public class EndlessHeliciteRounds : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 34;
			Item.damage = 20;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = 1;
			Item.consumable = false;
			Item.knockBack = 0f;
			Item.value = Item.sellPrice(platinum: 9);
			Item.shoot = ModContent.ProjectileType<HeliciteRoundProjectile>(); // The projectile that weapons fire when using this item as ammunition.
			Item.shootSpeed = 35f;
			Item.ammo = AmmoID.Bullet;
		}
	}
}