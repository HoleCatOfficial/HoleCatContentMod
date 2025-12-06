using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Tiles.Riftplate;

using Terraria.Localization;
using Terraria.Audio;
using DestroyerTest.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Tools;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftThrowingKnife : RechargeItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 30f;
			Item.shoot = ModContent.ProjectileType<RiftThrowingKnifeProjectile>();
			Item.width = 14;
			Item.height = 34;
			Item.maxStack = 100;
			Item.consumable = true;
			Item.UseSound = SoundID.Item71;
			Item.useAnimation = 8;
			Item.useTime = 8;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.damage = 60;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiscData>()
                .AddIngredient<Item_Riftplate>(10)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
        }
	}
}