using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class DTEclipseEffects : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return Main.eclipse && DTMusicConfig.instance.ReplaceVanillaTracks;
        }

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/SolarEclipse");

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
