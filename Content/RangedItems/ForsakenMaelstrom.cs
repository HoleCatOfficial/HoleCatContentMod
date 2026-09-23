using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
    public class ForsakenMaelstrom : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Contempt>();
            }
        }

        public override void SetDefaults()
        {
            Item.width = 118;
            Item.height = 44;
            Item.value = Item.sellPrice(gold: 35, silver: 72, copper: 6);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();

            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 10;
            Item.autoReuse = true;
            Item.damage = 116;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.crit = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;

            Item.shoot = ModContent.ProjectileType<ForsakenMaelstromHoldout>();
            Item.shootSpeed = 5f;
            //Item.useAmmo = AmmoID.Bullet;

        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.RemnantsIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<NightmareRoseArtifact>(3)
                    .Register();
            }
        }
    }
}