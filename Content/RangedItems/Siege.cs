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
	public class Siege : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
		}

		public override void SetDefaults()
        {
            // Common Properties
            Item.width = 34;
            Item.height = 42;
            Item.value = Item.sellPrice(gold: 35, silver: 72, copper: 6);
            Item.rare = ItemRarityID.Red;

            // Use Properties
            // Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
            // Each attack takes a different amount of time to execute
            // Conforming to the item useTime and useAnimation makes it much harder to design
            // It does, however, affect the item tooltip, so don't leave it out.
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;

            // Weapon Properties
            Item.knockBack = 10;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
            Item.autoReuse = true; // This determines whether the weapon has autoswing
            Item.damage = 4000; // The damage of your sword, this is dynamically adjusted in the projectile code.
            Item.DamageType = DamageClass.Ranged; // Deals melee damage\
            Item.channel = true;
            Item.crit = 16; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
            Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.useTurn = true;
            

            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<SiegeHoldout>(); // The sword as a projectile
        }

        public override bool CanUseItem(Player player)
        {
            bool Rocket = player.HasItemInAnyInventory(ItemID.RocketI) || player.HasItemInAnyInventory(ItemID.RocketII) || player.HasItemInAnyInventory(ItemID.RocketIII) || player.HasItemInAnyInventory(ItemID.RocketIV);
            return player.ownedProjectileCounts[Item.shoot] < 1 && Rocket;
        }
        

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.RocketLauncher, 1)
                .AddIngredient<Tenebris>(5)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}