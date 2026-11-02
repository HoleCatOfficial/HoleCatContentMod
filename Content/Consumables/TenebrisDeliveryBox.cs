using DestroyerTest.Common.DropRules;
using DestroyerTest.Content.Resources;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

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