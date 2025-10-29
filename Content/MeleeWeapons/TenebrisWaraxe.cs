using System;
using DestroyerTest.Content.Dusts;
  
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class TenebrisWaraxe : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.width = 70; // The item texture's width.
            Item.height = 70; // The item texture's height.

            Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
            Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
            Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
            Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
            Item.damage = 160; // The damage your item deals.
            Item.knockBack = 6; // The force of knockback of the weapon. Maximum is 20
            Item.crit = 20; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

            Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
            Item.rare = ModContent.RarityType<ShimmeringRarity>(); // Give this item our custom rarity.
            Item.shoot = ModContent.ProjectileType<TenebrisWaraxeProjectile>();
            Item.shootSpeed = 36f;
            Item.ArmorPenetration = 15;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}