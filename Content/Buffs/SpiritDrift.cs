using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    // This class serves as an example of a debuff that causes constant loss of life
    // See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
    public class SpiritDrift : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer<SDPlayer>(out var Drift))
            {
                Drift.Levitation = true;
            }
        }

        public override void Update(NPC target, ref int buffIndex)
        {
            if (target.TryGetGlobalNPC<SDTarget>(out var modNPC))
            {
                modNPC.Levitation = true;
            }
        }
    }

    public class SDTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool Levitation;

        public override void ResetEffects(NPC npc)
        {
            Levitation = false;
        }

        public override void PostAI(NPC npc)
        {
            // Call the custom Update logic
            Update(npc);
        }

        public void Update(NPC npc)
        {
            // Only apply the effects if Levitation is active
            if (Levitation && !npc.boss)
            {
                npc.velocity = new Vector2(npc.velocity.X / 2, -1.4f);


                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.BlueMoss, 0.0f, -0.5f, 0, default, 1);
                d.noGravity = true;
            }

        }
    }

    public class SDPlayer : ModPlayer
	{
		public bool Levitation;

		public override void ResetEffects()
		{
			Levitation = false;
		}

        public override void PostUpdateBuffs()
        {
            if (Levitation)
			{
                Player.velocity.Y = -4f;
                Player.velocity.X *= 0.99f;
			}
        }
		public override void UpdateBadLifeRegen()
		{
			if (Levitation)
			{
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				Player.lifeRegenTime = 0;
				Player.lifeRegen -= 6;
			}
		}
	}
}
