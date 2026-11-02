using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class Passion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetModPlayer<PassionPlayer>().Active = true;
        }
    }

    public class PassionPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {

            }
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (Active)
            {
                healValue = (int)(healValue * 1.2f);
            }
        }

    }
}