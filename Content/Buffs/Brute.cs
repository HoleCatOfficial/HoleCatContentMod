using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class Brute : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BrutePlayer>().Active = true;
            player.buffImmune[ModContent.BuffType<Enfeebled>()] = true;
        }
    }

    public class BrutePlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (item.DamageType == ModContent.GetInstance<DTTrueMeleeClass>())
            {
                if (Active)
                {
                    scale = 1.45f;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Active)
            {
                Player.GetDamage<DTTrueMeleeClass>() += 0.25f;
            }
        }
    }
}
