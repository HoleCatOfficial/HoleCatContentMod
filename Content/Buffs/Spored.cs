using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class Spored : ModBuff
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
			player.GetModPlayer<SporePlayer>().Spored = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<SporeTarget>(out var modNPC)) {
                modNPC.Spored = true;
            }
		}
	}
	
	public class SporeTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool Spored;

        public override void ResetEffects(NPC npc) {
            Spored = false;
        }

        public override void AI(NPC npc)
        {
			if (Spored)
			{
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GlowingMushroom, 0.0f, 0.0f, 0, default, 1);
				if (Main.rand.NextBool(4))
				{
					for (int g = 0; g < 8; g++)
					{
						//PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), npc.Center, Main.rand.NextVector2Circular(3, 3), new Color(63, 66, 207) * 0.5f, 1f);
						//PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), npc.Center, Main.rand.NextVector2Circular(3, 3), new Color(63, 66, 207), 0.5f);
					}
				}
				if (!npc.boss)
				{
					npc.velocity.X *= 0.95f;
				}
                npc.AddBuff(BuffID.Confused, 240);
			}
            base.AI(npc);
        }


        public void UpdateLifeRegen(NPC npc, Player player, ref int damage)
		{
			if (Spored)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 9;
			}
		}
    }

	public class SporePlayer : ModPlayer
	{
		public bool Spored;

		public override void ResetEffects()
		{
			Spored = false;
		}

        public override void PostUpdateBuffs()
        {
            if (Spored)
			{
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.GlowingMushroom, 0.0f, 0.0f, 0, default, 1);
                if (Main.rand.NextBool(4))
                {
                    for (int g = 0; g < 8; g++)
                    {
                        //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Player.Center, Main.rand.NextVector2Circular(3, 3), new Color(63, 66, 207) * 0.5f, 1f);
                        //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Player.Center, Main.rand.NextVector2Circular(3, 3), new Color(63, 66, 207), 0.5f);
                    }
                }
                Player.velocity.X *= 0.35f;
                Player.AddBuff(BuffID.Confused, 240);
			}
            base.PostUpdateBuffs();
        }
		public override void UpdateBadLifeRegen()
		{
			if (Spored)
			{
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				Player.lifeRegenTime = 0;
				Player.lifeRegen -= 6;
			}
		}
	}
}