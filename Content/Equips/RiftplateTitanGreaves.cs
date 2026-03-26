using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Legs)]
	public class RiftplateTitanGreaves : ModItem, IRechargeFunctionality
	{
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }
        
		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.defense = 10;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetArmorPenetration(DamageClass.Generic) += 2;

			if (Energized)
			{
				player.GetDamage(DamageClass.Generic) += 0.1f;
			}
		}

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Living_Shadow>(30)
                .AddIngredient<Item_Riftplate>(10)
                .AddTile<Tile_RiftConfiguratorArmory>()
                .Register();
		}
	}
}