using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace DestroyerTest.Common.DropRules
{
    public class EternityDropRuleCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

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

