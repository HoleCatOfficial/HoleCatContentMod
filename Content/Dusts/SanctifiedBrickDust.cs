using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
    public class SanctifiedBrickDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            return true;
        }

    }
}
