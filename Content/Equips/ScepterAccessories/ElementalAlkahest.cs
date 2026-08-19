
using System;
using DestroyerTest.Common;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
	[AutoloadEquip(EquipType.HandsOn)]
	public class ElementalAlkahest : ModItem
	{
		
		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 6);
			Item.rare = ItemRarityID.Green;
            Item.vanity = false;
            Item.accessory = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetAttackSpeed<ScepterClass>() += 0.05f;
            player.GetDamage<ScepterClass>() += 0.10f;
            player.ScepterClass().Range += 300;
		}

       public class EA_DROP_NPC : GlobalNPC
		{
			public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {

				if (npc.type == NPCID.MoonLordCore) {
					
				}
			}
		}
	}
}
