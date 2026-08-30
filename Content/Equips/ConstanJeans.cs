
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Legs)]
	public class ConstanJeans : ModItem
	{
        public override void SetStaticDefaults()
        {
            DTUtils.isDevItem[Type] = true;
        }

		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18; 
			Item.value = Item.sellPrice(gold: 2, silver: 35);
            Item.rare = ModContent.RarityType<DevRarity>();
			Item.defense = 27;
		}

		public override void UpdateEquip(Player player) 
		{
			player.buffImmune[BuffID.OnFire3] = true;
			player.GetCritChance(DamageClass.Generic) += 16;
		}


	}
}
