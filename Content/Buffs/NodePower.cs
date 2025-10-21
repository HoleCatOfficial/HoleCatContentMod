using System.Xml;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class NodePower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<NodePowerPlayer>().Empower = true;
		}

        // Allows you to make this buff give certain effects to the given player
        public override void Update(NPC target, ref int buffIndex)
        {
            if (target.TryGetGlobalNPC<NodePowerNPC>(out var modNPC))
            {
                modNPC.Empower = true;
            }
        }
    }

    public class NodePowerNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool Empower;

        public override void ResetEffects(NPC npc)
        {
            Empower = false;
        }

        public override void PostAI(NPC npc)
        {
            Update(npc);
        }

        public void Update(NPC npc)
        {
            if (Empower)
            {
                if (Empower && Main.GameUpdateCount % 10 == 0) // throttle particles
                {
                    PRTLoader.NewParticle(
                        PRTLoader.GetParticleID<BloomRingSharp>(),
                        npc.Center,
                        Vector2.Zero,
                        ColorLib.RainbowGradient,
                        0.005f
                    );
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player target, Item item, ref NPC.HitModifiers modifiers)
        {
            if (Empower)
                modifiers.FinalDamage *= 0.5f;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (Empower)
                modifiers.FinalDamage *= 1.75f;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Empower)
                npc.lifeRegen += 20;
        }
    }

    public class NodePowerPlayer : ModPlayer
    {

        // Flag checking when life regen debuff should be activated
        public bool Empower;

        public override void ResetEffects()
        {
            Empower = false;
        }

        public override void PostUpdateBuffs()
        {
            
            if (Empower && DTUtils.NodeCharmEquipped)
            {
                Player.GetDamage(DamageClass.Generic) *= 1.50f;
            }
        }

        public override void UpdateLifeRegen()
        {
            
            if (Empower && DTUtils.NodeCharmEquipped)
            {
                Player.lifeRegen += 15;
            }
        }
        
        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if (Empower && DTUtils.NodeCharmEquipped)
            {
                if (item == Player.HeldItem)
                {
                    crit += 16;
                }
            }
        }
	}
}
