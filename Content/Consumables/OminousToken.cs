
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.AchievementPaintingTiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;

namespace DestroyerTest.Content.Consumables
{
    
    public class OminousToken : ModItem
    {

        public override void SetStaticDefaults()
        {
            
            ItemID.Sets.OpenableBag[Type] = true;
            Item.ResearchUnlockCount = 5;
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
            // We have to replicate the expert drops from MinionBossBody here

            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<HekatesMystique>(), 10, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MalachiteKnives>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<TwistedFaith>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<FailedPotion>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GodGouger>(), 5, 1, 1));

            itemLoot.Add(ItemDropRule.Coins(1250, true));
        }
    }
}