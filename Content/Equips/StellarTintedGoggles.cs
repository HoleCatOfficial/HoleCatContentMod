
using DestroyerTest.Common;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	public class StellarTintedGoggles : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 29;
			Item.height = 18;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
			Item.rare = ModContent.RarityType<StellarRarity>();
		}

		

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			DTFlags.StellarGogglesEquipped = true;
		}

        public override void AddRecipes()
        {
            if (DTCrossMod.RemnantsIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<ConstitutionArtifact>(3)
                    .Register();
            }
        }
    }
}