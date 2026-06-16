
using DestroyerTest.Common;
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
        public override void SetStaticDefaults()
        {
            DTUtils.isDevItem.Add(Type);
        }

        public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 2);
            Item.rare = ModContent.RarityType<DevRarity>();
			Item.defense = 32;
            Item.vanity = false;
		}

		public override void UpdateEquip(Player player) 
		{
			player.buffImmune[BuffID.Electrified] = true;
			player.GetDamage(DamageClass.Generic) += 0.05f;
		}
	}
}
