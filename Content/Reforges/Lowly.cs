using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Reforges
{
	public class Lowly : ModPrefix
	{

		public override PrefixCategory Category => PrefixCategory.Custom;

		public override float RollChance(Item item) 
		{
			return 4f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		// Use this function to modify these stats for items which have this prefix:
		// Damage Multiplier, Knockback Multiplier, Use Time Multiplier, Scale Multiplier (Size), Shoot Speed Multiplier, Mana Multiplier (Mana cost), Crit Bonus.
		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			useTimeMult *= 1.5f;
            knockbackMult *= 1.75f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 0.25f;
		}

		public override void Apply(Item item) 
		{
			// Not much need to touch this.
		}

        /*
		public string ReforgeKey = "Mods.DestroyerTest.Reforges";
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "LowlyReforge", Language.GetTextValue($"{ReforgeKey}.Lowly")) 
			{
				IsModifier = true,
                IsModifierBad = true
			};
		}
        */

		public override void SetStaticDefaults() 
		{
		
		}
	}
}