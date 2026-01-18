using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class HeliciteHeadgear : ModItem
	{
		public override void SetStaticDefaults() {
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<RiftRarity2>(); // The rarity of the item
			Item.defense = 10; // The amount of defense the item will give when equipped
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<HeliciteRobe>() && legs.type == ModContent.ItemType<HeliciteChausses>();
		}

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(player.armor[0]);
            player.manaRegen += 40;
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(15)
				.AddIngredient<Item_HeliciteCrystal>(5)
				.AddIngredient<Item_RiftClay>(3)
                .AddIngredient(ItemID.Silk, 15)
				.AddTile<Tile_RiftConfiguratorArmory>()
				.Register();
		}
	}
}