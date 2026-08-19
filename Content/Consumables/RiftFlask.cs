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
	public class RiftFlask : ModItem
	{
		public override void SetStaticDefaults() {

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
			Item.buffType = ModContent.BuffType<WeaponImbueDO>();
			Item.buffTime = Item.flaskTime;
			Item.value = Item.sellPrice(0, 0, 5);
			Item.rare = ModContent.RarityType<RiftRarity1>();
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Item_HeliciteCrystal>(8)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}
	}
}