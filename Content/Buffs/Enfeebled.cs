using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class Enfeebled : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true; 
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<EnfeebledPlayer>().Debuff = true;
        }
    }

    public class EnfeebledPlayer : ModPlayer
    {
        public bool Debuff;

        public override void ResetEffects()
        {
            Debuff = false;
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (item.DamageType != DamageClass.Melee || item.DamageType != DamageClass.MeleeNoSpeed || item.DamageType != DamageClass.SummonMeleeSpeed)
            {
                return;
            }
            else
            {
                if (Debuff)
                {
                    scale = 0.5f;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Debuff)
            {
                Player.GetAttackSpeed(DamageClass.Melee) *= 0.5f;
                Player.GetAttackSpeed(DamageClass.MeleeNoSpeed) *= 0.5f;
                Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= 0.5f;
            }
        }
    }
}
