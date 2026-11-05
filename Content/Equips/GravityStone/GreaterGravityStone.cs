
using System;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Resources;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.GravityStone
{
    public class GreaterGravityStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.master = true;
            Item.masterOnly = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.gravity = Math.Max(player.gravity, 0.6f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LesserGravityStone>(1)
                .AddIngredient(ItemID.SoulofLight, 12)
                .AddIngredient<SunscorchedCinder>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}