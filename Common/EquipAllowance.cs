using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Equips;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public static class RecipeAccessoryConflicts
    {
        // Result -> All ingredients used to make it (recursive)
        public static readonly Dictionary<int, HashSet<int>> Dependencies = [];

        public static void Build()
        {
            Dependencies.Clear();

            foreach (Recipe recipe in Main.recipe)
            {
                if (recipe == null || recipe.createItem.IsAir)
                    continue;

                int result = recipe.createItem.type;

                if (!Dependencies.TryGetValue(result, out var set))
                {
                    set = [];
                    Dependencies[result] = set;
                }

                foreach (Item ingredient in recipe.requiredItem)
                {
                    if (ingredient == null || ingredient.IsAir)
                        continue;

                    set.Add(ingredient.type);
                }
            }

            // Expand transitively.
            bool changed;
            do
            {
                changed = false;

                foreach ((int result, HashSet<int> ingredients) in Dependencies)
                {
                    List<int> current = [.. ingredients];

                    foreach (int ingredient in current)
                    {
                        if (!Dependencies.TryGetValue(ingredient, out var subIngredients))
                            continue;

                        foreach (int sub in subIngredients)
                        {
                            if (ingredients.Add(sub))
                                changed = true;
                        }
                    }
                }
            }
            while (changed);
        }

        public static bool UsesIngredient(int result, int ingredient)
        {
            return Dependencies.TryGetValue(result, out var set) &&
                   set.Contains(ingredient);
        }
    }

    internal class RecipeConflictLoading : ModSystem
    {
        public override void PostAddRecipes()
        {
            RecipeAccessoryConflicts.Build();
        }
    }

    public class EquipAllowance : GlobalItem
    {
        public override bool InstancePerEntity => true;



        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            // Upgrade chain
            if (DTUtils.NoUpgradeStack[incomingItem.type] &&
                RecipeAccessoryConflicts.UsesIngredient(incomingItem.type, equippedItem.type))
            {
                return false;
            }

            if (DTUtils.NoUpgradeStack[incomingItem.type] &&
                RecipeAccessoryConflicts.UsesIngredient(equippedItem.type, incomingItem.type))
            {
                return false;
            }

            // Explicit blacklist
            if (DTUtils.NoEquipWith[incomingItem.type].Contains(equippedItem.type))
            {
                return false;
            }

            if (DTUtils.NoEquipWith[equippedItem.type].Contains(incomingItem.type))
            {
                return false;
            }

            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }
    }
}
