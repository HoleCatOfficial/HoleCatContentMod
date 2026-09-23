using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.SummonItems
{
    public class RiftBallBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<RiftBall>());
        }
    }
}