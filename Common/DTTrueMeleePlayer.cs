using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

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
            if (Player.setSolar && item.DamageType == ModContent.GetInstance<DTTrueMeleeClass>())
            {
                return 0.7f;
            }

            return 1f;
        }
    }
}
