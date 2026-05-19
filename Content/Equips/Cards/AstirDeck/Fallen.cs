
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
	public class Fallen : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 666;
			Item.accessory = true;
            Item.rare = ItemRarityID.Red;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            if (player.TryGetModPlayer<FallenPlayer>(out var Fall))
            {
                Fall.Active = true;
            }
		}
    }

    public class FallenDropNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.DemonTaxCollector)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Fallen>(), 10, 1, 1));
            }

        }
    }

    public class FallenPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }
        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (Active)
            {
                scale *= 1.225f;
            }
        }
    }
}