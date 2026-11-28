using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
    //See also
    //Example Drop Condition: https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Common/ItemDropRules/DropConditions/ExampleDropCondition.cs
    //Example Journey Mode Drop Condition: https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Common/ItemDropRules/DropConditions/ExampleJourneyModeDropCondition.cs
    public class EternityDropRuleCondition : IItemDropRuleCondition
    {
        public static LocalizedText Description;

        public EternityDropRuleCondition()
        {
            Description ??= Language.GetOrRegister("Mods.DestroyerTest.DropConditions.EternityMode");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return DestroyerTestMod.EternityIsActive || DestroyerTestMod.MasochistIsActive;
        }

        public bool CanShowItemDropInUI()
        {
            return DestroyerTestMod.EternityIsActive || DestroyerTestMod.MasochistIsActive;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}

