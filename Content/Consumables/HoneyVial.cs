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
	public class HoneyVial : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;
		}
		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 32;
			Item.DefaultToVial(ModContent.BuffType<ScepterImbueHoney>(), ModContent.RarityType<PearlRarity>(), Item.sellPrice(0, 0, 5));
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.BottledHoney, 4)
                .Register();
		}
	}
}