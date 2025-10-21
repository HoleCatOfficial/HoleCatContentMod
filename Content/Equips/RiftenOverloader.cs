
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Equips
{
	public class RiftenOverloader : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 29;
			Item.height = 18;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
		}

		

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.buffImmune[ModContent.BuffType<HeliouricShock>()] = true;
		}
	}
}