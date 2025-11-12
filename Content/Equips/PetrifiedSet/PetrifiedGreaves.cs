using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;

using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Equips.PetrifiedSet
{
	[AutoloadEquip(EquipType.Legs)]
	public class PetrifiedGreaves : ModItem
	{
		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 18;
			Item.value = DTUtils.GetScepterArmorSellPricePerRarity(Item.rare); 
			Item.rare = ModContent.RarityType<WineRarity>();
			Item.defense = 8;
		}

	}
}