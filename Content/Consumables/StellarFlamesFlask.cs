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
using Terraria.Audio;

namespace DestroyerTest.Content.Consumables
{
	public class StellarFlamesFlask : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				ColorLib.StellarColor
			];
		}

		public override void SetDefaults() {
			Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/StellarFlamesFlask") with { PitchVariance = 0.2f, MaxInstances = 0 };
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 22;
			Item.height = 34;
			Item.buffType = ModContent.BuffType<WeaponImbueGB>();
			Item.buffTime = Item.flaskTime;
			Item.value = Item.sellPrice(0, 2, 55);
			Item.rare = ModContent.RarityType<StellarRarity>();
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<StellarMatter>(8)
                .AddTile(TileID.ImbuingStation)
                .Register();
            Recipe.Create(ModContent.ItemType<GalantineVial>(), 1)
                .AddIngredient(Type)
                .Register();
		}
	}
}