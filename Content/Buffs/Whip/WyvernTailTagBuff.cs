using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Whip
{
    public class WyvernTailTagBuff : ModBuff
    {
        public static readonly int TagDamage = 30;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}