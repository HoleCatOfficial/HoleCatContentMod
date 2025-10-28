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
	public class GalantineVial : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				ColorLib.StellarColor
			];
		}

		public override void SetDefaults() {
			Item.UseSound = SoundID.Item3;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 22;
			Item.height = 32;
			Item.buffType = ModContent.BuffType<ScepterImbueGB>();
			Item.buffTime = Item.flaskTime;
			Item.value = Item.sellPrice(0, 2, 5);
            Item.rare = ItemRarityID.Blue;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<StellarMatter>(4)
                .AddTile(TileID.ImbuingStation)
                .Register();
            Recipe.Create(ModContent.ItemType<RiftFlask>(), 1)
                .AddIngredient(Type)
                .Register();
		}
	}
}