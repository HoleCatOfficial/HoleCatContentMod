using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee.Flail;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.Flails
{
	public class CoronaBreaker : ModItem
	{
		public override void SetStaticDefaults() 
        {

		}

        public int attackType = 1;
        public int hitcount = 0;
        public float p = 0f;

		public override void SetDefaults() 
        {
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.knockBack = 5.5f;
			Item.width = 66;
			Item.height = 46;
			Item.damage = 70;
			Item.noUseGraphic = true; 
			Item.shoot = ModContent.ProjectileType<CoronaBreakerProjectile>();
			Item.shootSpeed = 12f;
			Item.UseSound = SoundID.Item71;
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.value = Item.sellPrice(gold: 1, silver: 50);
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, ai1: attackType);
			attackType = attackType == 1 ? 2 : 1;
			return false;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(14)
                .AddIngredient(ItemID.Chain, 6)
				.AddTile<Tile_RiftConfiguratorWeaponry>()
				.Register();
		}
	}
}