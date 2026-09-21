using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Projectiles.Pets;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class InfernalBatBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<InfernalBat>());
        }
    }
}
