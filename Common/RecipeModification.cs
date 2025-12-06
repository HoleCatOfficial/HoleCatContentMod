using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;

namespace DestroyerTest.Common
{
    public class RecipeModification : ModSystem
    {
        public override void AddRecipes() {
			Recipe zenith = Recipe.Create(ItemID.Zenith, 1);

            Recipe clonedRecipe = zenith.Clone();
            clonedRecipe.AddIngredient(ItemID.TerraBlade);
            clonedRecipe.AddIngredient(ItemID.Meowmere);
            clonedRecipe.AddIngredient(ItemID.StarWrath);
            clonedRecipe.AddIngredient(ItemID.InfluxWaver);
            clonedRecipe.AddIngredient(ItemID.TheHorsemansBlade);
            clonedRecipe.AddIngredient(ItemID.Seedler);
            clonedRecipe.AddIngredient(ItemID.Starfury);
            clonedRecipe.AddIngredient(ItemID.BeeKeeper);
            clonedRecipe.AddIngredient(ItemID.EnchantedSword);
            clonedRecipe.AddIngredient(ItemID.CopperBroadsword);
            clonedRecipe.AddIngredient<Gargantua>();
            clonedRecipe.AddIngredient<Conclusion>();
			clonedRecipe.Register();
		}

    }
}