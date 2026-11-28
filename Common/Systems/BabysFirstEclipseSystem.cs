using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
    //Talid makes it so that you are guaranteed a Solar Eclipse on the first day after your first mech kill.
    //I was tired of my tester(s) skipping over the Rift content.
    public class BabysFirstEclipseSystem : ModSystem
    {
        public bool FirstEclipse = false;
        public override void PostUpdateTime()
        {
            if (Main.dayTime && NPC.downedMechBossAny)
            {
                if (!FirstEclipse)
                {
                    Main.eclipse = true;
                    FirstEclipse = true;
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("FirstEclipse", FirstEclipse);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("FirstEclipse"))
            {
                FirstEclipse = tag.GetBool("FirstEclipse");
            }
        }
    }


}
