using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Common.Systems;

namespace DestroyerTest.Content.Resources
{
	public class Living_Shadow : ModItem
	{
		public override void SetStaticDefaults() {
			// The text shown below some item names is called a tooltip. Tooltips are defined in the localization files. See en-US.hjson.

			// How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.wiki.gg/wiki/Journey_Mode#Research for a list of commonly used research amounts depending on item type. This defaults to 1, which is what most items will use, so you can omit this for most ModItems.
			Item.ResearchUnlockCount = 500;

			// This item is a custom currency (registered in ExampleMod), so you might want to make it give "coin luck" to the player when thrown into shimmer. See https://terraria.wiki.gg/wiki/Luck#Coins
			// However, since this item is also used in other shimmer related examples, it's commented out to avoid the item disappearing
			//ItemID.Sets.CoinLuckValue[Type] = Item.value;
		}

		public override void SetDefaults() {
			Item.width = 20; // The item texture's width
			Item.height = 20; // The item texture's height

			Item.maxStack = Item.CommonMaxStack; // The item's max stack value
			Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }
	}
    public class LS_DROP_NPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) 
		{
			if (npc.type == NPCID.Eyezor || 
                npc.type == NPCID.Frankenstein || 
                npc.type == NPCID.SwampThing ||
                npc.type == NPCID.Vampire ||
                npc.type == NPCID.CreatureFromTheDeep ||
                npc.type == NPCID.Fritz ||
                npc.type == NPCID.ThePossessed ||
                npc.type == NPCID.Reaper ||
                npc.type == NPCID.Mothron ||
                npc.type == NPCID.MothronSpawn ||
                npc.type == NPCID.Butcher ||
                npc.type == NPCID.DeadlySphere ||
                npc.type == NPCID.DrManFly ||
                npc.type == NPCID.Nailhead ||
                npc.type == NPCID.Psycho) {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Living_Shadow>(), 5, 10, 40));
			}
		}
	}
}

