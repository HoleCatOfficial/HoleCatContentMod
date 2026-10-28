using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Whip
{
    public class HeliciteWhipTagBuff : ModBuff
    {
        public static readonly int TagDamage = 28;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}