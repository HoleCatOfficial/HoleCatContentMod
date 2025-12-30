using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
	public class ComaceraticFlask : ModItem
	{
		public override void SetStaticDefaults() 
		{
			ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<ComaceraticVial>()] = Type;
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				ColorLib.Rift,
				ColorLib.DarkRift3,
				ColorLib.LightRift2
			];
		}

		public override void SetDefaults() 
		{
			Item.width = 22;
			Item.height = 32;
			Item.DefaultToFlask(ModContent.BuffType<WeaponImbueCB>(), ModContent.RarityType<RiftRarity1>(), Item.sellPrice(0, 0, 5));
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Item_HeliciteCrystal>(8)
                .AddIngredient(ItemID.FragmentSolar, 3)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}
	}
}