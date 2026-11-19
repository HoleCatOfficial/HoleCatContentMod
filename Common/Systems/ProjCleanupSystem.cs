using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Entities;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class ProjCleanupSystem : ModSystem
    {

    }

    public class ProjCleanup : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool PreAI(Projectile projectile)
        {
            if (DTUtils.OwnedByBossNPC[projectile.type] > -1)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.type == DTUtils.OwnedByBossNPC[projectile.type] && !n.active)
                    {
                        if (n.type == ModContent.NPCType<WyvernCorpseHead>() && DestroyerTestMod.EternityIsActive)
                        {
                            SunlightModification.Reset();
                        }
                        projectile.Kill();
                    }

                }
            }
            return base.PreAI(projectile);
        }
    }
}
