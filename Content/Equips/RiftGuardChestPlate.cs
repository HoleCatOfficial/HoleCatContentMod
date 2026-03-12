using rail;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.RiftArsenal;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Body)]
	public class RiftGuardChestPlate : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<RiftRarity1>(); // The rarity of the item
			Item.defense = 35; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			bool Set = player.armor[0].type == ModContent.ItemType<RiftGuardVisor>() && player.armor[2].type == ModContent.ItemType<RiftGuardChausses>();
			if (!Set)
			{
				player.GetCritChance(DamageClass.Ranged) += 40;
			}
			else
			{
				player.GetCritChance(DamageClass.Ranged) += 25;
			}

			if (Energized)
			{
				player.GetArmorPenetration(DamageClass.Ranged) += 6;
			}
		}


		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(24)
				.AddIngredient<ShadowCircuitry>(6)
                .AddIngredient(ItemID.PalladiumBar, 14)
				.AddTile<Tile_RiftConfiguratorArmory>()
				.Register();
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(24)
				.AddIngredient<ShadowCircuitry>(6)
                .AddIngredient(ItemID.CobaltBar, 14)
				.AddTile<Tile_RiftConfiguratorArmory>()
				.Register();
		}
	}
}