using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
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
	public class GalantineHoneyVial : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;

            ItemID.Sets.DrinkParticleColors[Type] = [
                Color.Gold,
                ColorLib.StellarColor
			];
		}
		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 32;
			Item.DefaultToVial(ModContent.BuffType<ScepterImbueGH>(), ModContent.RarityType<StellarRarity>(), Item.sellPrice(0, 0, 5));
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HoneyVial>()
                .AddIngredient<StellarMatter>(4)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}
	}
}