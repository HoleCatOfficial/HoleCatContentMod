
using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.GravityStone
{
    public class LesserGravityStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.WyvernHead, ItemDropRule.ByCondition(new Conditions.IsMasterMode(), Type, 10));
            ItemID.Sets.ItemNoGravity[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.master = true;
            Item.masterOnly = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.gravity = Math.Max(player.gravity, 0.4f);
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                CreateRecipe()
                    .AddIngredient(ItemID.SoulofFlight, 20)
                    .AddIngredient(ItemID.StoneBlock, 5)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
    }
}