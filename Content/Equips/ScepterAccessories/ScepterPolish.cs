
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
	public class ScepterPolish : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 86);
			Item.rare = ModContent.RarityType<PearlRarity>();
            Item.vanity = false;
            Item.accessory = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.ScepterClass().Range += 30;
		}
	}
}
