using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Magic.StarWand;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
    public class WhisperingTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.White;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 70;
            Item.autoReuse = false;
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<WhisperingTomeHoldout>();
            Item.channel = true;
            Item.mana = 20;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient<SunscorchedCinder>(4)
                .AddIngredient<Living_Shadow>(22)
                .Register();
        }
    }
}
