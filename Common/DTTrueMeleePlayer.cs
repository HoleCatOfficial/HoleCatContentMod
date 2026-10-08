using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MonoMod.RuntimeDetour;

namespace DestroyerTest.Common
{
    public class DTTrueMeleePlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (Player.setSolar)
            {
                Player.GetDamage<DTTrueMeleeClass>() += 0.3f;
                Player.GetCritChance<DTTrueMeleeClass>() += 16f;
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            // 211 = Feral Claws
            // Player.AutoReuseGlove = Feral Claws and upgrades to them.
            // 

            // 536 = Titan Glove
            // Player.kbGlove and Player.MeleeScaleGlove = Titan Glove and upgrades.

            // 897 = Power Glove
            // Power Glove doesnt do anything exclusive, so it just sets the previous 3 flags to true and adds the 12% speed boost from the feral claws.

            // 936 = Mechanical Glove
            // All of Power Glove's effects unchanged, but with an additional 12% boost to melee damage.

            // 1343 = Fire Gauntlet
            // All of Mechanical Glove's effects unchanged, but also sets Player.MagmaStone to true for its fire effect.

            // 3992 = Berserker's Glove
            // All of Power Glove's effects unchanged, but with Player.aggro increased by 400.


            if (item.DamageType.CountsAsClass<DTTrueMeleeClass>())
            {
                //Feral Claws
                if (Player.autoReuseGlove)
                {
                    return 0.88f;
                }

                if (Player.setSolar)
                {
                    return 0.7f;
                }
            }
            return 1f;
        }

        public override bool? CanAutoReuseItem(Item item)
        {
            if (item.DamageType.CountsAsClass<DTTrueMeleeClass>() && (Player.autoReuseGlove || Player.autoReuseAllWeapons))
            {
                return true;
            }
            return null;
        }

        public override void ModifyWeaponKnockback(Item item, ref StatModifier knockback)
        {
            if (item.DamageType.CountsAsClass<DTTrueMeleeClass>())
            {
                if (Player.kbGlove)
                {
                    knockback *= 2;
                }
            }
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (item.DamageType.CountsAsClass<DTTrueMeleeClass>())
            {
                if (Player.kbGlove)
                {
                    scale += 0.1f;
                }
            }
        }
    }
}
