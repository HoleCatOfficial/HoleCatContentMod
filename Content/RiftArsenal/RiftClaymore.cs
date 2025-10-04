  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria.Audio;
using DestroyerTest.Content.Tools;
using DestroyerTest.Common;
using System.Collections.Generic;

using Terraria.Localization;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.Tiles.RiftConfigurator;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftClaymore : RechargeItem
	{
		public override void SetDefaults() {
			// Common Properties
			Item.width = 46;
			Item.height = 48;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<RiftRarity1>();

			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/ManaBurst") with { PitchVariance = 0.2f };

			// Weapon Properties
			Item.knockBack = 7;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			Item.damage = 88; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = DamageClass.Melee; // Deals melee damage
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<RiftClaymoreSlash>(); // The sword as a projectile
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }


		public override bool MeleePrefix()
		{
			return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Energized)
			{
				type = ModContent.ProjectileType<RiftClaymoreSlashEnergized>();
			}
			else
			{
				type = ModContent.ProjectileType<RiftClaymoreSlash>();
			}

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false; // return false so vanilla doesn't also spawn the default projectile
		}




		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<GreatSwordData>()
				.AddIngredient<Item_Riftplate>(28)
				.AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
		}
	}
}