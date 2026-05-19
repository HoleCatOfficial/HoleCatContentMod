using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;

namespace DestroyerTest.Common.BalanceUtils
{

    public class BalanceTimeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        float Time = 0f;
        public override void SetDefaults(NPC entity)
        {
            
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.boss)
            {
                Time = 0f;
                Main.NewText($"Boss Logging Initiated for: {npc.TypeName}");
            }
        }
        public override void AI(NPC npc)
        {
            Time += 1f / 60f;
        }

        public override void OnKill(NPC npc)
        {
            if (!npc.boss)
                return;

            string folder = Main.SavePath + "/BossTimes";

            Directory.CreateDirectory(folder);

            string path = Path.Combine(
                folder,
                $"{npc.TypeName}.dat"
            );

            File.AppendAllText(path, Time.ToString("0.00") + Environment.NewLine);
        }
    }
}
