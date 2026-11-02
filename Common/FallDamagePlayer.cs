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
