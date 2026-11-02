using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class DTBloodMoonEffects : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return Main.bloodMoon && DTMusicConfig.instance.ReplaceVanillaTracks;
        }

        int Tier()
        {
            if (NPC.downedGolemBoss)
            {
                return 3;
            }
            if (NPC.downedMechBossAny)
            {
                return 2;
            }
            return 1;
        }
        public override int Music => MusicLoader.GetMusicSlot($"DestroyerTest/Assets/Music/BloodMoon/BloodMoonTier{Tier()}");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override float GetWeight(Player player)
        {
            return 0.9f;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {

        }
    }
}
