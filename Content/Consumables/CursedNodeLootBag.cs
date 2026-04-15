
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.AchievementPaintingTiles;
using DestroyerTest.Content.Tools;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class CursedNodeLootBag : ModItem
    {

        public override void SetStaticDefaults()
        {
            
            ItemID.Sets.BossBag[Type] = true;
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
        }


        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<HaepienNodeCharm>(), 24, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<WretchedIdol>(), 1, 1, 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<WretchedShards>(), 1, 4, 16));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedFlameScroll>(), 1, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Malevolence>(), 2, 1, 1));
            itemLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Item_CursedFlameNodeRelic>()));
            itemLoot.Add(ItemDropRule.Common(ItemID.FlaskofCursedFlames, 3, 1, 9));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ItemID.CursedFlame, 2, 20, 60));
            itemLoot.Add(ItemDropRule.Coins(1250, true));
        }
    }
}