using DestroyerTest.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class ComaceraticBurn : ModBuff
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
			player.GetModPlayer<CBPlayer>().lifeRegenDebuff = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<CBTarget>(out var modNPC)) {
                modNPC.lifeRegenDebuff = true;
            }
		}
	}
	
	public class CBTarget : GlobalNPC
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
				Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<RiftDust>(), 0.0f, 0.5f, 0, default, 1);
                if (npc.boss == false)
                {
                    npc.velocity *= 0.65f;
                }
			}
            base.AI(npc);
        }


        public void UpdateLifeRegen(NPC npc, Player player, ref int damage)
		{
			if (lifeRegenDebuff && Main.dayTime)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<RiftDust>(), 0.0f, 0.5f, 0, default, 1);
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;
				npc.lifeRegen -= 36;
			}
			if (lifeRegenDebuff && !Main.dayTime)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 40;
			}
		}
    }

	public class CBPlayer : ModPlayer
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
				Player.lifeRegen -= 32;
			}
		}
	}
}