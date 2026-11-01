using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class FallDamagePlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (DTVanillaChangesConfig.instance.NaturalFallDamageImmunity)
            {
                Player.noFallDmg = true;
            }
        }
    }
}
