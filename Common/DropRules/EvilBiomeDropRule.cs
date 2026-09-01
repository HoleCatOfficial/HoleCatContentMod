using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
    public class EvilBiomeDropRule : IItemDropRuleCondition
    {
        public static LocalizedText Description;

        bool crimson = true;

        public EvilBiomeDropRule(bool Crimson) 
        { 
            crimson = Crimson;
            Description ??= Language.GetOrRegister("Mods.DestroyerTest.DropConditions.EvilBiome");
        }

        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
        {
            return crimson ? WorldGen.crimson : !WorldGen.crimson;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI()
        {
            return true;
        }

        string IProvideItemConditionDescription.GetConditionDescription()
        {
            return Description.Value;
        }
    }
}
