
using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	public class HaepienNodeCharm : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 42;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			DTUtils.NodeCharmEquipped = true;
		}
	}
}