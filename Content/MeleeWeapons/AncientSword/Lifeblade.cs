using DestroyerTest;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.AncientSword
{
    [AutoloadGlowmask]
    public class Lifeblade : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 370;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.shoot = ModContent.ProjectileType<LifebladeSwing>();
            Item.noUseGraphic = true;
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
            //Might + Sight
            CreateRecipe()
                .AddIngredient<LostWonder>()
                .AddIngredient(ItemID.LifeFruit, 4)
                .AddIngredient(ItemID.SoulofMight, 2)
                .AddIngredient(ItemID.SoulofSight, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            //Might + Fright
            CreateRecipe()
                .AddIngredient<LostWonder>()
                .AddIngredient(ItemID.LifeFruit, 4)
                .AddIngredient(ItemID.SoulofMight, 2)
                .AddIngredient(ItemID.SoulofFright, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            //Fright + Sight
            CreateRecipe()
                .AddIngredient<LostWonder>()
                .AddIngredient(ItemID.LifeFruit, 4)
                .AddIngredient(ItemID.SoulofFright, 2)
                .AddIngredient(ItemID.SoulofSight, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}