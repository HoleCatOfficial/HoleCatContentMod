using System;
using System.Collections.Generic;
using DestroyerTest.Common;
  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.RiftBiome;

using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tools;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftRevolver : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }

        public override void SetDefaults() {
			// Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// Common Properties
			Item.width = 44; // Hitbox width of the item.
			Item.height = 26; // Hitbox height of the item.
			Item.scale = 0.75f;
			Item.rare = ModContent.RarityType<RiftRarity1>(); // The color that the item's name will be in-game.

			// Use Properties
			Item.useTime = 10; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 10; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

			// The sound that this item plays when used.
			Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/OB_Shot") {
				Volume = 0.6f,
				PitchVariance = 0.2f,
				MaxInstances = 3,
			};

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
			Item.damage = 60; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			Item.shootSpeed = 15f; // The speed of the projectile (measured in pixels per frame.) This value equivalent to Handgun
			Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RevolverData>()
                .AddIngredient<ShadowCircuitry>(6)
                .AddIngredient<Item_Riftplate>(12)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
        }

		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.rand.NextBool(4) && Energized)
			{
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RiftStarFriendly>(), damage + 15, knockback, player.whoAmI, ai2: 1);
			}
			else
			{
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			}
			return false;
        }

	}
}