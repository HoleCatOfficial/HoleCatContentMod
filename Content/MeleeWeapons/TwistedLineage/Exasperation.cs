using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using System;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.MeleeWeapons.TwistedLineage
{
    public class Exasperation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword.Add(Type);
            DTUtils.TooltipScaleMult[Type] = 1.22f;
        }

        public override void SetDefaults()
        {
            Item.width = 96;
            Item.height = 96;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.White;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.damage = 500;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ExasperationSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TwilightInferno>()
                .AddIngredient<ShadeParticle>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}