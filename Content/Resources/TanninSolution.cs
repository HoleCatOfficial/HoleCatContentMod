  
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Resources
{
	public class TanninSolution : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 3;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 26;
			Item.value = 20;
            Item.consumable = true;
			Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.UseSound = SoundID.Item3;
		}

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.Poisoned, 600);
        }

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater, 1)
                .AddIngredient(ItemID.Acorn, 2)
				.AddTile(TileID.Campfire)
				.Register();
		}
	}
}