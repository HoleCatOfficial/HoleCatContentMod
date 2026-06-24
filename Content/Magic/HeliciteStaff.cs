using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tools;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
	public class HeliciteStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.staff[Type] = true;
		}

		public override void SetDefaults() 
		{

			Item.shoot = ModContent.ProjectileType<HeliciteStaffHoldout>();
            Item.useTime = 90;
            Item.useAnimation = 90;
			Item.width = 92;
			Item.height = 92;
			Item.autoReuse = true;
			Item.crit = 12;
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.channel = true;
			Item.InterruptChannelOnHurt = false;
			Item.StopAnimationOnHurt = false;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.DamageType = DamageClass.Magic;
            Item.mana = 60;
            Item.damage = 150;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
		
		
		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RiftStaff>()
                .AddIngredient<Item_HeliciteCrystal>(15)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
            .Register();
        }
	}
}