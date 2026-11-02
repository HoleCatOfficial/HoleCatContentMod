using DestroyerTest.Content.Equips.PotionFlowers;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.ThrowerItems;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

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
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<UnstableSeedPouch>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<NeglectedRegards>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<FailedPotion>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GodGouger>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Dyrn>(), 5, 10, 30));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<BundleOfMagicLillies>(), 5, 1, 1));

            itemLoot.Add(ItemDropRule.Coins(1250, true));
        }
    }
}