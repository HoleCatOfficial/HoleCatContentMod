using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class DTRogueClass : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic)
                return new StatInheritanceData(1f, 1f);

            return new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
            );
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            
			if (damageClass == DamageClass.Throwing)
				return true;

            return false;
        }

        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<DTRogueClass>() += 4;
            player.GetAttackSpeed<DTRogueClass>() += 01f;
            player.GetArmorPenetration<DTRogueClass>() += 2;
        }
        public override bool UseStandardCritCalcs => true;
    }
}