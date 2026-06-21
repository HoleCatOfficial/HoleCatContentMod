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
    public class Blitz : ModItem
    {
        public override void SetStaticDefaults()
        {

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
            Item.damage =  40;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.crit = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;

            Item.shoot = ModContent.ProjectileType<BlitzHoldout>();
            Item.shootSpeed = 5f;
            //Item.useAmmo = AmmoID.Bullet;

        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Megashark, 1)
                .AddIngredient<PrimalShards>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}