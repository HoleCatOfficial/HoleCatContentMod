using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Imbues
{
    public abstract class BaseImbueBuff : ModBuff
    {
        public abstract WeaponImbuePlayer.Imbues Imbue { get; }
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsAFlaskBuff[Type] = true;
            Main.meleeBuff[Type] = true;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer<WeaponImbuePlayer>(out var imbuePlayer))
            {
                imbuePlayer.currentImbue = Imbue;
            }
            player.MeleeEnchantActive = true;
        }
    }
}
