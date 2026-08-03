using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace DestroyerTest.Content.Buffs
{
	public class SoulErosion : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  // Is it a debuff?
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
			BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
		}

		// Allows you to make this buff give certain effects to the given player
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SEPlayer>().Eroding = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<SETarget>(out var modNPC)) {
                modNPC.Eroding = true;
            }
		}
	}
	
	public class SETarget : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool Eroding;
        public float ErosionTimer;
        private int baseDamage;

        public override void SetDefaults(NPC npc)
        {
            baseDamage = npc.damage;
        }

        public override void ResetEffects(NPC npc)
        {
            Eroding = false;
        }

        public override void AI(NPC npc)
        {
            if (Eroding)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.SnowflakeIce, 0f, 0.5f);
                ErosionTimer += 1f;
                float reductionFactor = 1f - (0.05f * (ErosionTimer / 60f));
                int newDamage = (int)(baseDamage * MathHelper.Clamp(reductionFactor, 0.1f, 1f));
                npc.damage = Math.Max(newDamage, 2);
            }
            else
            {
                // reset timer if not actively eroding
                ErosionTimer = 0f;
                npc.damage = baseDamage;
            }
        }
    }


	public class SEPlayer : ModPlayer
    {
        public bool Eroding;
        public float ErosionTimer;

        public override void ResetEffects()
        {
            Eroding = false;
        }

        public override void PostUpdateBuffs()
        {
            if (Eroding)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.SnowflakeIce, 0f, 0.5f);
                ErosionTimer++;
            }
            else
            {
                ErosionTimer = 0f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Eroding)
            {
                float reductionFactor = 1f - (0.05f * (ErosionTimer / 60f));
                reductionFactor = MathHelper.Clamp(reductionFactor, 0.1f, 1f);
                modifiers.FinalDamage *= reductionFactor;
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Eroding)
            {
                float reductionFactor = 1f - (0.05f * (ErosionTimer / 60f));
                reductionFactor = MathHelper.Clamp(reductionFactor, 0.1f, 1f);
                modifiers.FinalDamage *= reductionFactor;
            }
        }
    }

}