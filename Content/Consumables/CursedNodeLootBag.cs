using DestroyerTest.Rarity;
using Terraria;
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

        }
    }
}