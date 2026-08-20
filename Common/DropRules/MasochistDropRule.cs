using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
    public class MasochistDropRuleCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public MasochistDropRuleCondition()
        {
            Description ??= Language.GetOrRegister("Mods.DestroyerTest.DropConditions.MasochistMode");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return DestroyerTestMod.MasochistIsActive;
        }

        public bool CanShowItemDropInUI()
        {
            return DestroyerTestMod.MasochistIsActive;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}

