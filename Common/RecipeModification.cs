using DestroyerTest.Content.Fargos.Scepter;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.MeleeWeapons.TwistedLineage;
using DestroyerTest.Content.OrionCrossover;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class RecipeModification : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe recipeInk = Recipe.Create(ItemID.BlackInk);
            recipeInk.AddIngredient<Soot>(4);
            recipeInk.AddIngredient<MineralOil>(1);
            recipeInk.AddTile(TileID.WorkBenches);
            recipeInk.Register();
        }

        public override void PostAddRecipes() 
        {
            List<int> PillarArmor = new List<int>
            {
                ItemID.SolarFlareHelmet,
                ItemID.SolarFlareBreastplate,
                ItemID.SolarFlareLeggings,
                ItemID.VortexHelmet,
                ItemID.VortexBreastplate,
                ItemID.VortexLeggings,
                ItemID.StardustHelmet,
                ItemID.StardustBreastplate,
                ItemID.StardustLeggings,
                ItemID.NebulaHelmet,
                ItemID.NebulaBreastplate,
                ItemID.NebulaLeggings
            };

            foreach (Recipe recipe in Main.recipe)
            {
                if (recipe.HasResult(ItemID.Zenith))
                {
                    if (ModLoader.HasMod("CalamityMod") && ModLoader.TryGetMod("CalamityMod", out Mod Calamity))
                    {
                        if (Calamity.TryFind<ModItem>("AuricBar", out ModItem AuricBar) && Calamity.TryFind<ModTile>("CosmicAnvil", out ModTile CosmicAnvil))
                        {
                            if (recipe.HasIngredient(AuricBar.Type))
                            {
                                recipe.RemoveIngredient(AuricBar.Type);
                                recipe.AddIngredient(ItemID.LunarBar, 5);
                            }

                            if (recipe.HasTile(CosmicAnvil.Type))
                            {
                                recipe.RemoveTile(CosmicAnvil.Type);
                                recipe.AddTile(TileID.MythrilAnvil);
                            }
                        }
                    }

                    recipe.AddIngredient<Gargantua>();
                    recipe.AddIngredient<Committment>();
                    recipe.AddIngredient<SoulEdge>();
                    recipe.AddIngredient<RiftHypersabre>();
                    recipe.AddIngredient<Exasperation>();
                }

                foreach (int A in PillarArmor)
                {
                    if (recipe.HasResult(A) && recipe.HasIngredient(ItemID.LunarBar))
                    {
                        recipe.RemoveIngredient(ItemID.LunarBar);
                    }
                }

                if (recipe.HasResult<Sabhati>())
                {
                    if (DTCrossMod.OrionIsLoaded)
                    {
                        if (DTCrossMod.OrionMod.TryFind<ModItem>("HeliosSword", out var S))
                        {
                            recipe.AddIngredient(S.Type, 1);
                        }
                    }
                    else
                    {
                        recipe.DisableRecipe();
                    }
                }

                if (recipe.HasResult<GaiaScepter>())
                {
                    if (DTCrossMod.FargosSoulsIsLoaded)
                    {
                        bool B1 = DTCrossMod.FargosSoulsMod.TryFind<ModItem>("GaiaHelmet", out var gaiaH);
                        bool B2 = DTCrossMod.FargosSoulsMod.TryFind<ModItem>("DeviatingEnergy", out var DE);
                        if (B1 && B2)
                        {
                            recipe.AddIngredient(DE.Type, 50);
                            recipe.AddIngredient(gaiaH.Type);
                        }
                    }
                    else
                    {
                        recipe.DisableRecipe();
                    }
                }
            }
		}

    }
}