using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class Warped : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed -= Opus.Sine(0.3f, 0f);
            player.wingTimeMax = (int)(player.wingTimeMax * 1.5f);
            player.GetDamage(DamageClass.Throwing) += 0.08f;
        }
    }
}