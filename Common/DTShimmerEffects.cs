using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class DTShimmerEffects : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return player.ZoneShimmer;
        }

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/DTShimmerMusic");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        public override float GetWeight(Player player)
        {
            return 0.9f;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {

        }
    }
}
