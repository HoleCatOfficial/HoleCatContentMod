using System;
  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
	public class IchorDisruptor : ModItem
	{
		public override void SetDefaults() 
		{
            Item.width = 124;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 3, silver: 22);
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();

            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 10;
            Item.autoReuse = true;
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.crit = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;

            Item.shoot = ModContent.ProjectileType<IchorDisruptorHoldout>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Ichor, 6)
                .AddIngredient(ItemID.TitaniumBar, 6)
                .AddIngredient(ItemID.Boomstick)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}