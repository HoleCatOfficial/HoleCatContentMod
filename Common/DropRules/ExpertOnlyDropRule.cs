using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
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

