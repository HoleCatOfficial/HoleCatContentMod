using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
	public class FrostFireFlask : ModItem
	{
		public override void SetStaticDefaults() {
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<FrostFireVial>()] = Type;
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				Color.SkyBlue
			];
		}

		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 32;
			Item.DefaultToFlask(ModContent.BuffType<WeaponImbueFF>(), ItemRarityID.White, Item.sellPrice(0, 0, 5));
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Shiverthorn, 4)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}
	}
}