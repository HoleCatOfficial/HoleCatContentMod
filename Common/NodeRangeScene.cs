using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using DestroyerTest.Content.Entities;

namespace DestroyerTest.Common
{
    public class NodeRangeScene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            foreach(NPC npc in Main.npc)
            {
                if (npc.active && player.Distance(npc.Center) < 1200)
                {
                    if (npc.type == ModContent.NPCType<CursedFlameNodeMB>() || npc.type == ModContent.NPCType<IchorNodeMB>() || npc.type == ModContent.NPCType<BlessedNodeMB>())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override int Music => DTMusicConfig.instance.NodeIdleMusic ? MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/NodeIdle") : -1;

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    }
}
