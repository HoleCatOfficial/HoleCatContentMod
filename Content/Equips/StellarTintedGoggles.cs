
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips
{
	public class StellarTintedGoggles : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 29;
			Item.height = 18;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
			Item.rare = ModContent.RarityType<StellarRarity>();
		}

		

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			DTFlags.StellarGogglesEquipped = true;
		}
	}
}