
using DestroyerTest.Common;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
	public class SpookyScroll3 : LateHardmodeScroll
	{

		public override void SetDefaults() {
			Item.width = 32; // Width of the item
			Item.height = 30; // Height of the item
            Item.accessory = true;
		}

		public override void UpdateEquip(Player player) {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
            {
                Scptr.SpookyScroll3 = true;
            }
		}
	}
}
