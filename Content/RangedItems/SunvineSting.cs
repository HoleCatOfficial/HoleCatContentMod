using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles;

namespace DestroyerTest.Content.RangedItems
{
    public class SunvineSting : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public int HitCount = 0;
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 42;
            Item.value = Item.sellPrice(gold: 35, silver: 72, copper: 6);
            Item.rare = ModContent.RarityType<RiftRarity2>();

            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 10;
            Item.autoReuse = true;
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.crit = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;

            Item.shoot = ModContent.ProjectileType<SunvineStingHoldout>();
            Item.shootSpeed = 5f;
            //Item.useAmmo = AmmoID.Bullet;

        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public int ResetTime = 120;
        public override void UpdateInventory(Player player)
        {
            if (ResetTime > 0)
            {
                ResetTime--;
            }
            else
            {
                HitCount = 0;
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LifeFruit, 3)
                .AddIngredient<Item_HeliciteCrystal>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}