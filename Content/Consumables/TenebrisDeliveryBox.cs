
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Tiles.AchievementPaintingTiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Localization;
using DestroyerTest.Content.Resources;
using OpusLib.Content.Helpers;
using DestroyerTest.Common.DropRules;

namespace DestroyerTest.Content.Consumables
{
    public class TenebrisDeliveryBox : ModItem
    {

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.CultistBoss, ItemDropRule.ByCondition(new EvilBiomeDropRule(true), Type));
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.consumable = true;
            Item.width = 36;
            Item.height = 34;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Tenebris>(), 1, 25, 40));
        }
    }
}