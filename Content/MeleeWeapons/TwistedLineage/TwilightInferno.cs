using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;  
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.TwistedLineage
{
    [AutoloadGlowmask]
    public class TwilightInferno : ModItem
	{

        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1.15f;
        }

        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.White;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.damage = 97;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 22;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<TwilightInfernoSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DarkFireSword>()
                .AddIngredient(ItemID.ShadowFlameKnife)
                .AddIngredient<CarbonizedFlesh>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
} 