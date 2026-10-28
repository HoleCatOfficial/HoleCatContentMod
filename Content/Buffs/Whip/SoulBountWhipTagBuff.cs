using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Whip
{
    public class SoulBoundWhipTagBuff : ModBuff
    {
        public static readonly int TagDamage = 37;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}