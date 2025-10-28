
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
	public class SpookyScroll1 : ModItem
	{

		public override void SetDefaults() {
			Item.width = 32; // Width of the item
			Item.height = 30; // Height of the item
			Item.value = Item.sellPrice(gold: 86); // How many coins the item is worth
			Item.rare = ModContent.RarityType<PearlRarity>(); // The rarity of the item
            Item.vanity = false;
            Item.accessory = true;
		}

		public override void UpdateEquip(Player player) {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.SpookyScroll1 = true;
                }
            }
		}
	}
}
