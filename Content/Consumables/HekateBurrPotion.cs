using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
	public class HekateBurrPotion : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				new Color(21, 7, 38),
				new Color(54, 18, 101),
				new Color(125, 73, 192)
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
			Item.width = 20;
			Item.height = 30;
			Item.buffType = ModContent.BuffType<HekateBurrBuff>();
			Item.buffTime = (60 * 60) * 8;
			Item.value = Item.sellPrice(0, 2, 5);
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.ThornsPotion)
                .AddIngredient<Dyrn>(10)
                .AddTile(TileID.Bottles)
                .Register();
		}
	}
}