using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Social.Steam;

namespace DestroyerTest.Content.Resources
{

	public class Shadowflame : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 5));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 34;

			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(silver: 1);
		}




		public class SF_DROP_NPC : GlobalNPC
		{
			public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
			{
				if (
					npc.type == NPCID.GoblinSorcerer ||
					npc.type == NPCID.ShadowFlameApparition ||
					npc.type == NPCID.GoblinSummoner)
				{
					npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Shadowflame>(), 3, 5, 13));
				}

			}
		}
	}
}

