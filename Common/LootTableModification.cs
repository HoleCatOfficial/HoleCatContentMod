using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Scepter;

namespace DestroyerTest.Common
{
    public class TreasureBagModification : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.FrozenCrate)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrigidScroll>(), 3, 1, 1));
            }
            if (item.type == ItemID.LockBox)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnchantedScepter>(), 5, 1, 1));
            }
            if (item.type == ItemID.MoonLordBossBag && (!DownedBossSystem.downedNightmareRoseBoss && !DownedBossSystem.downedWyvernCorpseBoss))
            {
                itemLoot.RemoveWhere(rule =>
                {
                    if (rule is ItemDropWithConditionRule dropRule)
                    {
                        return dropRule.itemId == ItemID.Meowmere && dropRule.itemId == ItemID.LastPrism && dropRule.itemId == ItemID.RainbowCrystalStaff && dropRule.itemId == 3569 && dropRule.itemId == ItemID.StarWrath && dropRule.itemId == ItemID.Celeb2 && dropRule.itemId == ItemID.Terrarian && dropRule.itemId == ItemID.SDMG; //3569 is Lunar Portal Staff
                    }
                    return false;
                }, true);
            }
        }
    }

    public class NPCLootTableModification : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Deerclops)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HandScroll>(), 15));
            }
            if (npc.type == NPCID.CursedSkull)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CurseScroll>(), 50));
            }
        }
    }
}
