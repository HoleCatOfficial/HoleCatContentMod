
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Body)]
	public class CoatStantine : ModItem
	{
	

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ModContent.RarityType<DevRarity>();
			Item.defense = 32;
            Item.vanity = false;
		}

		public override void UpdateEquip(Player player) {
			player.buffImmune[BuffID.Electrified] = true;
			player.GetDamage(DamageClass.Generic) += 5f;
		}
	}
}
