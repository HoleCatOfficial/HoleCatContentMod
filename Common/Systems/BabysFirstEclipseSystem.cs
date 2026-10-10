using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

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
    }
}
