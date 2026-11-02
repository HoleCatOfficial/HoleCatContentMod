using DestroyerTest.Content.Projectiles.player.Accessory;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{

    public class UrcerisMiniBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool b1 = player.ownedProjectileCounts[ModContent.ProjectileType<UrcerisMini>()] > 0;
            if (b1)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}