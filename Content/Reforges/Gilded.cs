using System.Collections.Generic;
using FargowiltasSouls.Content.Projectiles.Souls;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Reforges
{
	public class Gilded : ModPrefix
	{

		public override PrefixCategory Category => PrefixCategory.Custom;

		public override float RollChance(Item item) 
		{
			return 1f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		// Use this function to modify these stats for items which have this prefix:
		// Damage Multiplier, Knockback Multiplier, Use Time Multiplier, Scale Multiplier (Size), Shoot Speed Multiplier, Mana Multiplier (Mana cost), Crit Bonus.
		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1.1f;
            knockbackMult *= 0.8f;
            shootSpeedMult *= 0.8f;
            critBonus /= 2;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1.7f;
		}

		public override void Apply(Item item) 
		{
			// Not much need to touch this.
		}

        /*
		public string ReforgeKey = "Mods.DestroyerTest.Reforges";
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "GildedReforge", Language.GetTextValue($"{ReforgeKey}.Gilded")) 
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