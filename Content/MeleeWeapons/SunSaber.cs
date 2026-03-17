using DestroyerTest;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class SunSaber : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 180;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 70);
            Item.rare = ItemRarityID.Master;
            Item.shoot = ModContent.ProjectileType<SunSaberSwing>();
            Item.noUseGraphic = true;
            Item.channel = true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.OrangePhasesaber, 1)
                .AddIngredient(ItemID.FragmentSolar, 24)
                .Register();
        }
    }
}