  
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
using Terraria.Localization;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using DestroyerTest.Content.Tools;
using Terraria.Audio;

using System.Collections.Generic;
using DestroyerTest.Content.Resources.Blueprints;

namespace DestroyerTest.Content.RiftArsenal
{
	// ExampleCustomSwingSword is an example of a sword with a custom swing using a held projectile
	// This is great if you want to make melee weapons with complex swing behavior
	public class RiftScythe : RechargeItem
	{

		public override void SetDefaults() {
			// Common Properties
			Item.width = 46;
			Item.height = 48;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<RiftRarity1>();

			// Use Properties
			// Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
			// Each attack takes a different amount of time to execute
			// Conforming to the item useTime and useAnimation makes it much harder to design
			// It does, however, affect the item tooltip, so don't leave it out.
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item71;

			// Weapon Properties
			Item.knockBack = 7;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			Item.damage = 100; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = DamageClass.Magic; // Deals melee damage
            Item.mana = 10;
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<RiftScytheSwing>(); // The sword as a projectile
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);
		
			return false; // return false to prevent original projectile from being shot
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BroadswordData>()
                .AddIngredient<ShadowCircuitry>(2)
                .AddIngredient<Item_Riftplate>(16)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
        }
	}
}