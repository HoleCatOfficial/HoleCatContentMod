using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
    //See also
    //Example Drop Condition: https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Common/ItemDropRules/DropConditions/ExampleDropCondition.cs
    //Example Journey Mode Drop Condition: https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Common/ItemDropRules/DropConditions/ExampleJourneyModeDropCondition.cs
    public class ExpertOnlyDropRuleCondition : IItemDropRuleCondition
    {
        public static LocalizedText Description;

        public ExpertOnlyDropRuleCondition()
        {
            Description ??= Language.GetOrRegister("Mods.DestroyerTest.DropConditions.ExpertOnly");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return Main.expertMode;
        }

        public bool CanShowItemDropInUI()
        {
            return Main.expertMode;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }

    public class NotExpertDropRuleCondition : IItemDropRuleCondition
    {
        public static LocalizedText Description;

        public NotExpertDropRuleCondition()
        {
            Description ??= Language.GetOrRegister("Mods.DestroyerTest.DropConditions.NotExpert");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return !Main.expertMode;
        }

        public bool CanShowItemDropInUI()
        {
            return !Main.expertMode;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}

