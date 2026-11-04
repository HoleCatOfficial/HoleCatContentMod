using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace DestroyerTest.Content.Buffs
{
    public class Stocky : ModBuff
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
            player.GetModPlayer<StockyPlayer>().Active = true;
            
            player.buffImmune[BuffID.Weak] = true;
        }
    }

    public class StockyPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void UpdateLifeRegen()
        {
            if (Active)
            {
                Player.lifeRegen += 8;
            }
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            base.ModifyMaxStats(out health, out mana);

            if (Active)
            {
                health += 0.3f;
                mana += 0;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Active)
            {
                modifiers.Knockback *= 0.5f;
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                hurtInfo.Knockback *= 0.5f;
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Active)
            {
                Player.statDefense += 2;
            }
        }
    }
}
