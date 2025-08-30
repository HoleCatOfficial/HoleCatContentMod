using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class SoulEffectPlayer : ModPlayer
    {
        public bool WyvernSoul = false;
        public bool RoseSoul = false;
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;
            if (RoseSoul)
            {
                health = StatModifier.Default;
                health.Base = 240;
            }
            if (WyvernSoul)
            {
                health = StatModifier.Default;
                health.Base = 360;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (RoseSoul)
            {
                Player.maxRunSpeed *= 1.45f;
            }
        }
    }
}