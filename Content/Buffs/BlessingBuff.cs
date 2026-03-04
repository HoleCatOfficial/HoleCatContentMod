using DestroyerTest.Common.Blessings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class BlessingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public string BlessingName = "";
        public string BonusText = "";
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer<PrayerPlayer>(out var PPlayer))
            {
                BonusText = PPlayer.CurrentBlessing.BlessingBonus;
                BlessingName = PPlayer.CurrentBlessing.BlessingName;
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Expert;

            if (BlessingName != "")
            {
                buffName = BlessingName;
            }
            if (BonusText != "")
            {
                tip = BonusText;
            }
        }
    }
}
