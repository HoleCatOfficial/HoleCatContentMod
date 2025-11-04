
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
	[AutoloadEquip(EquipType.HandsOn)]
	public class TemporalGlove : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 86);
			Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.vanity = false;
            Item.accessory = true;
		}

		public override void UpdateEquip(Player player) {
            player.GetDamage<ScepterClass>() *= 1.03f;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.TemporalGlove = true;
                }
            }
		}
	}
}
