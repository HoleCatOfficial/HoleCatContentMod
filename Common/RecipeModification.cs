using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using System;
using System.Collections.Generic;
using DestroyerTest.Content.Resources;

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
			clonedRecipe.Register();

            Recipe recipe = Recipe.Create(ItemID.BlackInk);
			recipe.AddIngredient<Soot>(4);
            recipe.AddIngredient<MineralOil>(1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();

            static Func<Recipe, bool> Vanilla(int itemID) => r => r.Mod is null && r.HasResult(itemID);
            static Action<Recipe> AddIngredient(int itemID, int stack = 1) => r => r.AddIngredient(itemID, stack);
            static Action<Recipe> RemoveIngredient(int itemID) => r => r.RemoveIngredient(itemID);
            var edits = new Dictionary<Func<Recipe, bool>, Action<Recipe>>(9999)
            {
                { Vanilla(ItemID.SolarFlareHelmet), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.SolarFlareBreastplate), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.SolarFlareLeggings), RemoveIngredient(ItemID.LunarBar) },

                { Vanilla(ItemID.VortexHelmet), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.VortexBreastplate), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.VortexLeggings), RemoveIngredient(ItemID.LunarBar) },

                { Vanilla(ItemID.StardustHelmet), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.StardustBreastplate), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.StardustLeggings), RemoveIngredient(ItemID.LunarBar) },
            
                { Vanilla(ItemID.NebulaHelmet), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.NebulaBreastplate), RemoveIngredient(ItemID.LunarBar) },
                { Vanilla(ItemID.NebulaLeggings), RemoveIngredient(ItemID.LunarBar) },


                { Vanilla(ItemID.Zenith), AddIngredient(ModContent.ItemType<Gargantua>()) },
            };
		}

    }
}