using System;
using System.Collections.Generic;
using DestroyerTest.Common;
  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
 

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
	public class RiftZapinator : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }
        public override void SetDefaults()
		{
			// Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// Common Properties
			Item.width = 46; // Hitbox width of the item.
			Item.height = 22; // Hitbox height of the item.
			Item.scale = 0.75f;
			Item.rare = ModContent.RarityType<RiftRarity1>(); // The color that the item's name will be in-game.

			// Use Properties
			Item.useTime = 40; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 40; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

			// The sound that this item plays when used.
			Item.UseSound = SoundID.Item33;

			// Weapon Properties
			Item.DamageType = DamageClass.Magic; // Sets the damage type to ranged.
			Item.mana = 10;
			Item.damage = 80; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<RiftLaser>(); // For some reason, all the guns in the vanilla source have this.
			Item.shootSpeed = 22f; // The speed of the projectile (measured in pixels per frame.) This value equivalent to Handgun
		}

		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(16)
				.AddIngredient<ShadowCircuitry>(7)
				.AddTile<Tile_RiftConfigurator>()
				.Register();
		}
	}
}
