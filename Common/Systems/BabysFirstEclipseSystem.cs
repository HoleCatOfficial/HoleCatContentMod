using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
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
