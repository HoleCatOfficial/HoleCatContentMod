using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
	public class RiftMaker : ModItem
	{
		public override void SetStaticDefaults() {
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 12f;
			Item.shoot = ModContent.ProjectileType<RiftMaker_Thrown>();
			Item.width = 14;
			Item.height = 56;
			Item.maxStack = 1;
			Item.UseSound = SoundID.Item71;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.damage = 80;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<DTRogueClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Item.StealthStrike(player))
            {
                for (int c = 0; c < 3; c++)
                {

                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, velocity.RotatedByRandom(0.3f), ModContent.ProjectileType<RiftStarFriendly>(), (int)(Item.damage * 0.5f), 1, player.whoAmI);

                }
            }

            return true;
        }

		public override void AddRecipes() 
		{
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(5)
				.AddIngredient<Living_Shadow>(5)
				.AddTile<Tile_RiftConfigurator>()
				.Register();
		}
	}
}