using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.RiftArsenal;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Legs)]
	public class RiftGuardChausses : RechargeItem
	{


		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18; 
			Item.value = Item.sellPrice(gold: 1); 
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.defense = 15;
		}

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer<RiftGuardChaussesRunBonus>(out var Bonus))
			{
				Bonus.Active = true;
				if (Energized)
				{
					Bonus.Charged = true;
				}
			}
        }


		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(16)
				.AddIngredient<ShadowCircuitry>(6)
                .AddIngredient(ItemID.PalladiumBar, 10)
				.AddTile<Tile_RiftConfiguratorArmory>()
				.Register();
			CreateRecipe()
				.AddIngredient<Item_Riftplate>(16)
				.AddIngredient<ShadowCircuitry>(6)
                .AddIngredient(ItemID.CobaltBar, 10)
				.AddTile<Tile_RiftConfiguratorArmory>()
				.Register();
		}
	}

	public class RiftGuardChaussesRunBonus : ModPlayer
	{
		public bool Active = false;
		public bool Charged = false;

        public override void ResetEffects()
        {
            Active = false;
			Charged = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Active)
			{
				if (Charged)
				{
					Player.runAcceleration *= 1.2f;
				}
				Player.maxRunSpeed *= 1.22f;
			}
        }


	}
}