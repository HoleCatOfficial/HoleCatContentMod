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
	public class HeliouricVial : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<HeliouricFlask>()] = Type;
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				ColorLib.Rift,
				ColorLib.DarkRift3,
				ColorLib.LightRift2
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
			Item.buffType = ModContent.BuffType<ScepterImbueHSk>();
			Item.buffTime = Item.flaskTime;
			Item.value = Item.sellPrice(0, 2, 5);
			Item.rare = ModContent.RarityType<RiftRarity1>();
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient<Living_Shadow>(180)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}
	}
}