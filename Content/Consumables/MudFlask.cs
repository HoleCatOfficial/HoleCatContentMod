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
	public class MudFlask : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				Color.Brown
			];
		}

		public override void SetDefaults() 
		{
			Item.width = 22;
			Item.height = 32;
			Item.DefaultToFlask(ModContent.BuffType<WeaponImbueMud>(), ItemRarityID.White, Item.sellPrice(0, 0, 5));
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.MudBlock, 8)
                .Register();
		}
	}
}