using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Consumables;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Entities;

namespace DestroyerTest.Content.Buffs
{
	public class ArenaEffects: ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = false;  // Is it a debuff?
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			var mp = player.GetModPlayer<ApplyArenaEffectsPlayer>();
			if (mp.CurrentArenaBoss == ModContent.NPCType<NightmareRoseBoss>())
				mp.NightmareRose = true;
		}
	}

	public class ApplyArenaEffectsPlayer : ModPlayer
	{
        public bool BuffActive = false;
		public int CurrentArenaBoss = -1;
		public bool NightmareRose;

		public override void ResetEffects() {
			NightmareRose = false;
		}

        public override void PostUpdateBuffs() {
			if (NightmareRose) {
				float windForce = 0.2f;

				// Push like wind, but cap only the *wind’s contribution*
				if (Player.velocity.X < 2f && windForce > 0) {
					Player.velocity.X += windForce;
				}
				else if (Player.velocity.X > -3f && windForce < 0) {
					Player.velocity.X += windForce;
				}
			}
		}
	}
}