using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;

namespace DestroyerTest.Content.RangedItems
{
	public class Dreampoon : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
		}

		public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 42;
            Item.value = Item.sellPrice(gold: 35, silver: 72, copper: 6);
            Item.rare = ItemRarityID.Red;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;
            Item.autoReuse = true;
            Item.damage = 4000;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.crit = 16;
            Item.noMelee = true; 
            Item.noUseGraphic = true;
            Item.useTurn = true;

            Item.shoot = ModContent.ProjectileType<DreampoonHoldout>(); // The sword as a projectile
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 16)
                .AddIngredient<Vesper>(5)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}