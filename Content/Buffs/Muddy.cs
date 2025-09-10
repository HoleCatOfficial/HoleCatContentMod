using DestroyerTest.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class Muddy : ModBuff
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
			player.GetModPlayer<MudPlayer>().Muddy = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<MudTarget>(out var modNPC)) {
                modNPC.Muddy = true;
            }
		}
	}
	
	public class MudTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool Muddy;

        public override void ResetEffects(NPC npc) {
            Muddy = false;
        }

        public override void AI(NPC npc)
        {
			if (Muddy)
			{
                if (!npc.boss)
                {
                    npc.velocity *= 0.5f;
                }
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Mud, 0.0f, -1f, 0, default, 1);
			}
            base.AI(npc);
        }
    }

	public class MudPlayer : ModPlayer
	{
		public bool Muddy;

		public override void ResetEffects()
		{
			Muddy = false;
		}

        public override void PostUpdateBuffs()
        {
            if (Muddy)
            {
                Player.moveSpeed *= 0.5f;
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Mud, 0.0f, -1f, 0, default, 1);
			}
            base.PostUpdateBuffs();
        }
	}
}