
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	// This class serves as an example of a debuff that causes constant loss of life
	// See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
	public class ShimmeringFlames : ModBuff
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
			player.GetModPlayer<SFPlayer>().lifeRegenDebuff = true;
		}
		
		public override void Update(NPC target, ref int buffIndex)
		{
			if (target.TryGetGlobalNPC<SFTarget>(out var modNPC))
			{
				modNPC.lifeRegenDebuff = true;
			}
		}
	}

	public class SFTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc) {
            lifeRegenDebuff = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            if (lifeRegenDebuff) {
            

				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -0.1f), ColorLib.TenebrisGradient * 0.35f, 1.0f, 40, ai2: 2);
				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, Color.White, 0.25f);
				}

                if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

                npc.lifeRegen -= 32;
            }
        }
    }

	public class SFPlayer : ModPlayer
	{

		// Flag checking when life regen debuff should be activated
		public bool lifeRegenDebuff;

		public override void ResetEffects() {
			lifeRegenDebuff = false;
		}

		// Allows you to give the player a negative life regeneration based on its state (for example, the "On Fire!" debuff makes the player take damage-over-time)
		// This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
		// The player will take damage at a rate of half the number you subtract per second
		public override void UpdateBadLifeRegen() {
			Player player = Main.LocalPlayer;
			if (lifeRegenDebuff)
			{
				

				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(player.Hitbox), new Vector2(0f, -0.1f), ColorLib.TenebrisGradient * 0.35f, 1f, 40, ai2: 2);
				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Main.rand.NextVector2FromRectangle(player.Hitbox), Vector2.Zero, Color.White, 0.5f);
				} 
				// These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				// Player.lifeRegenTime used to increase the speed at which the player reaches its maximum natural life regeneration
				// So we set it to 0, and while this debuff is active, it never reaches it
				Player.lifeRegenTime = 0;
				// lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
				Player.lifeRegen -= 28;
			}
		}
	}
}