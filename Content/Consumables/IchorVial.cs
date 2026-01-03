using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
	public class IchorVial : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.ShimmerTransformToItem[ItemID.FlaskofIchor] = Type;
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				ColorLib.Ichor
			];
		}

		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 32;
			Item.DefaultToVial(ModContent.BuffType<ScepterImbueIchor>(), ModContent.RarityType<WineRarity>(), Item.sellPrice(0, 0, 5));
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Ichor, 8)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}
	}
}