using ConstellationsOfOrion.Content.Items.Weapons;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.OrionCrossover;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.OrionCrossover
{
    public class Sabhati : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 160;
            Item.height = 160;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 70;
            Item.autoReuse = false;
            Item.damage = 200;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SabhatiSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HeliosSword>(1)
                .AddIngredient<Constitution>(1)
                .AddIngredient(ItemID.Starfury, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}