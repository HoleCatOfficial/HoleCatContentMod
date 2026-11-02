using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class StardustSummonBoost : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) += 0.05f;
        }
    }
}