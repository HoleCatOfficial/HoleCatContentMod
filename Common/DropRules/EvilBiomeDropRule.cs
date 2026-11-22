using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
    //See also
    //Example Drop Condition: https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Common/ItemDropRules/DropConditions/ExampleDropCondition.cs
    //Example Journey Mode Drop Condition: https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Common/ItemDropRules/DropConditions/ExampleJourneyModeDropCondition.cs
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
