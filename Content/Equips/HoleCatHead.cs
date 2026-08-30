

using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{

	[AutoloadEquip(EquipType.Head)]
	public class HoleCatHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
			DTUtils.isDevItem[Type] = true;

        }
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 36; 
			Item.value = Item.sellPrice(gold: 70);
            Item.rare = ModContent.RarityType<DevRarity>();
            Item.defense = 23;
		}

		
		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<HoleCatBody>() && legs.type == ModContent.ItemType<HoleCatLegs>();
		}


		public override void UpdateArmorSet(Player player)
		{
			player.GetDamage(DamageClass.Generic) += 1.14f;
			player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
			player.GetKnockback(DamageClass.Generic) += 1.1f;
			player.maxTurrets += 3;

			if (DTCrossMod.CalamityIsLoaded)
			{
				DTCrossMod.CalamityMod.Call("AddMaxStealth", player, 0.2f);

                DTCrossMod.CalamityMod.Call("SetWearingRogueArmor", player, true);
            }
			player.DefaultSetBonusText(Item);
		}
	}
}
