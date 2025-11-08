
using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class WeaponImbueGB : ModBuff
	{
		public override void SetStaticDefaults() {
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<WeaponImbuePlayer>().GalantineBurn = true;
			player.MeleeEnchantActive = true;
		}
	}
}