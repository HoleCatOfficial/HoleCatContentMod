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
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Body)]
	public class HeliciteRobe : ModItem
	{
        public int equipBack = -1;

        public override void Load()

        {
            if (Main.netMode != NetmodeID.Server)
            {
                equipBack = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = equipBack;
        }

        public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<HeliciteRarity>();
			Item.defense = 35;
		}
        
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(60)
				.AddIngredient<Item_HeliciteCrystal>(30)
				.AddIngredient<Item_RiftClay>(22)
                .AddIngredient(ItemID.Silk, 15)
				.AddTile<Tile_RiftConfigurator>()
				.Register();
		}
	}
}