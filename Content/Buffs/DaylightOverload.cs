using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class DaylightOverload : ModBuff
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
			player.GetModPlayer<DOPlayer>().lifeRegenDebuff = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<DOTarget>(out var modNPC)) {
                modNPC.lifeRegenDebuff = true;
            }
		}
	}
	
	public class DOTarget : GlobalNPC
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
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.FireworksRGB, 0.0f, 0.5f, 0, ColorLib.Rift, 1);
				if (npc.boss == false)
                {
                    npc.velocity *= 0.65f;
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
				npc.lifeRegen -= 36;
			}
		}
    }

	public class DOPlayer : ModPlayer
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
				Dust.NewDust(Player.position, Player.width, Player.height, DustID.FireworksRGB, 0.0f, 0.5f, 0, ColorLib.Rift, 1);
				Player.moveSpeed *= 0.85f;
			}
            base.PostUpdateBuffs();
        }
		public override void UpdateBadLifeRegen()
		{
			if (lifeRegenDebuff)
			{
				Player.lifeRegenTime = 0;
				Player.lifeRegen -= 16;
			}
		}
	}
}