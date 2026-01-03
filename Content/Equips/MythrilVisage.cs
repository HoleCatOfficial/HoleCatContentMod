
﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class MythrilVisage : ModItem
	{
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<WineRarity>();
			Item.defense = 10;
            Item.vanity = true;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves;
		}

		public static readonly int SoloRangeBonus = 10;
		public static readonly int SetRangeBonus = 18;
        public static readonly float ThrowSpeedBonus = 1.12f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SoloRangeBonus);
		public LocalizedText setBonus => base.Tooltip.WithFormatArgs(SetRangeBonus, (ThrowSpeedBonus - 1f).ToString("P1"));
		public override void UpdateArmorSet(Player player) {
			ScepterClassStats.Range += SetRangeBonus;
			ScepterClassStats.ThrowSpeedModifier *= ThrowSpeedBonus;
			player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.MythrilVisage.SetBonus", setBonus);
		}

        public override void UpdateEquip(Player player)
        {
            ScepterClassStats.Range += SoloRangeBonus;
        }


		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient(ItemID.MythrilBar, 10)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
