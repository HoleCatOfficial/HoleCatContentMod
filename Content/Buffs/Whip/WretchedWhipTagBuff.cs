using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Whip
{
    public class WretchedWhipTagBuff : ModBuff
    {
        public static readonly int TagDamage = 13;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}