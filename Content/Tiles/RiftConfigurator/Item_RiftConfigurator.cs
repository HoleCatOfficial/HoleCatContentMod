using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RiftConfigurator
{
	public class Item_RiftConfigurator : ModItem
	{
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 25));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tile_RiftConfigurator>());
			Item.height = 64;
			Item.width = 46;
			Item.value = 6000;
			Item.rare = ModContent.RarityType<RiftRarity1>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wire, 16)
				.AddIngredient<Motherboard>(6)
				.AddIngredient<ShadowCircuitry>(8)
				.AddIngredient<Item_Riftplate>(24)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}

	}

}
