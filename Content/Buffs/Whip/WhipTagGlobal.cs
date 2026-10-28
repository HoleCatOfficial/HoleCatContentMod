using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Whip
{
    public class WhipTagGlobal : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
            {
                return;
            }

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<WyvernTailTagBuff>())
            {
                modifiers.FlatBonusDamage += WyvernTailTagBuff.TagDamage * projTagMultiplier;
            }

            if (npc.HasBuff<WretchedWhipTagBuff>())
            {
                modifiers.FlatBonusDamage += WretchedWhipTagBuff.TagDamage * projTagMultiplier;
            }

            if (npc.HasBuff<SoulBoundWhipTagBuff>())
            {
                modifiers.FlatBonusDamage += SoulBoundWhipTagBuff.TagDamage * projTagMultiplier;
            }

            if (npc.HasBuff<HeliciteWhipTagBuff>())
            {
                modifiers.FlatBonusDamage += HeliciteWhipTagBuff.TagDamage * projTagMultiplier;
            }

            if (npc.HasBuff<RiftWhipTagBuff>())
            {
                modifiers.FlatBonusDamage += RiftWhipTagBuff.TagDamage * projTagMultiplier;
            }
        }
    }
}
