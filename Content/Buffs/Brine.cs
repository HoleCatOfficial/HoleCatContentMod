using DestroyerTest.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class Brine : ModBuff
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
			player.GetModPlayer<BrinePlayer>().lifeRegenDebuff = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<BrineTarget>(out var modNPC)) {
                modNPC.lifeRegenDebuff = true;
            }
		}
	}
	
	public class BrineTarget : GlobalNPC
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
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Water_Snow, 0.0f, -1f, 0, default, 1);
			}
            base.AI(npc);
        }


        public void UpdateLifeRegen(NPC npc, Player player, ref int damage)
		{
			if (lifeRegenDebuff)
			{
				
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 19;
			}
		}
    }

	public class BrinePlayer : ModPlayer
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
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Water_Snow, 0.0f, -1f, 0, default, 1);
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
				Player.lifeRegen -= 12;
			}
		}
	}
}