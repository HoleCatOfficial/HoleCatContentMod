using DestroyerTest.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.MeleeWeapons
{

	public class Horizon : ModItem
	{
		public override void SetDefaults() {
            Item.height = 39;
            Item.width = 39;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Shoot;
			
			Item.shoot = ModContent.ProjectileType<ConstitutionSwing>();
			

			Item.damage = 20; 
			Item.shootSpeed = 1;
		}

		
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Goliath>()
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}