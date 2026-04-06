using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.Buffs
{
	public class HeliouricShock : ModBuff
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
			player.GetModPlayer<HSkPlayer>().lifeRegenDebuff = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<HSkTarget>(out var modNPC)) {
                modNPC.lifeRegenDebuff = true;
            }
		}
	}
	
	public class HSkTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc) {
            lifeRegenDebuff = false;
        }

        public override void AI(NPC npc)
        {
            if (lifeRegenDebuff)
            {
                int[] types = new int[]
                {
                    PRTLoader.GetParticleID<Arc1>(),
                    PRTLoader.GetParticleID<Arc2>(),
                    PRTLoader.GetParticleID<Arc3>()
                };

				Vector2 pos = npc.Center + Main.rand.NextVector2Circular(npc.width * 0.5f, npc.height * 0.5f);
				if (Main.rand.NextBool(20) && !DTOptimizationsConfig.instance.DisableExcessParticles)
				{
					PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], pos, Vector2.Zero, ColorLib.Rift, 0.3f);
				}

				//Dust.NewDust(npc.position, npc.width, npc.height, DustID.Lava, 0.0f, 0.5f, 0, default, 1);
				if (npc.boss == false)
                {
                    npc.velocity *= 0.95f;
                }

                foreach (Player plr in Main.player)
					{
						if (plr.Center.Distance(npc.Center) < 40)
						{
							plr.AddBuff(ModContent.BuffType<HeliouricShock>(), 120);
						}
					}
			}
            base.AI(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (lifeRegenDebuff)
			{
				if (npc.lifeRegen > 0)
				{
                    npc.lifeRegen = 0;
				}
				npc.lifeRegen -= 20;
			}
		}
    }

	public class HSkPlayer : ModPlayer
	{
		public bool lifeRegenDebuff;

		public override void ResetEffects()
		{
			lifeRegenDebuff = false;
		}

        public override void PostUpdateBuffs()
        {
			if (lifeRegenDebuff)
			{
                int[] types = new int[]
                {
                    PRTLoader.GetParticleID<Arc1>(),
                    PRTLoader.GetParticleID<Arc2>(),
                    PRTLoader.GetParticleID<Arc3>()
                };

				Vector2 pos = Main.rand.NextVector2Circular(Player.width * 0.5f, Player.height * 0.5f);

				if (Main.rand.NextBool(20) && !DTOptimizationsConfig.instance.DisableExcessParticles)
				{
					PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], pos, Vector2.Zero, ColorLib.Rift, 0.25f);
				}
				Player.moveSpeed *= 0.85f;
			}
            base.PostUpdateBuffs();
        }
		public override void UpdateBadLifeRegen()
		{
			if (lifeRegenDebuff)
			{
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				Player.lifeRegenTime = 0;
				Player.lifeRegen -= 8;
			}
		}
	}
}