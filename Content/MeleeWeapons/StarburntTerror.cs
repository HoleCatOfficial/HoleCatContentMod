using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.UI;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class StarburntTerror : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 116;
            Item.height = 106;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 110;
            Item.knockBack = 4f;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 16);
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<StarburntTerrorSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
        }


        public override void AddRecipes()
        {
            /*
            CreateRecipe()
                .AddIngredient<Constitution>()
                .AddIngredient(ItemID.TheHorsemansBlade)
                .AddIngredient(ItemID.Pumpkin, 10)
                .AddIngredient<Living_Shadow>(8)
                .AddIngredient<SunscorchedCinder>(4)
                .AddIngredient(ItemID.SoulofNight, 8)
                .Register();
            */
        }


    }
}
